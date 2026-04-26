from flask import Flask, render_template, request, redirect, url_for
import json
import math
import os
import re
import sqlite3
from collections import Counter, defaultdict

app = Flask(__name__)

DB_PATH = os.path.join(os.path.dirname(__file__), "app.db")

MASS_VACANCIES = ["Кассир", "Сборщик заказов", "Кладовщик"]
PRO_VACANCIES = ["Python-разработчик", "Маркетолог", "Аналитик данных"]
INTERNSHIP_VACANCIES = ["Стажер в IT", "Стажер в маркетинге", "Стажер в логистике"]

APPLICATION_FORMATS = {
    "full_time_prof": "Полная занятость проф вакансий",
    "full_time_mass": "Полная занятость массовых вакансий",
    "internship_prof": "Стажировка проф вакансий",
}

ROLE_DOMAIN = {
    "Кассир": "retail",
    "Сборщик заказов": "logistics",
    "Кладовщик": "logistics",
    "Python-разработчик": "it",
    "Маркетолог": "marketing",
    "Аналитик данных": "analytics",
    "Стажер в IT": "it",
    "Стажер в маркетинге": "marketing",
    "Стажер в логистике": "logistics",
}

PROFILE_MODEL = None


def get_db_connection():
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def init_db():
    conn = get_db_connection()
    cur = conn.cursor()
    cur.execute(
        """
        CREATE TABLE IF NOT EXISTS users (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            type TEXT NOT NULL,
            username TEXT NOT NULL,
            password TEXT NOT NULL,
            access_key TEXT
        )
        """
    )
    cur.execute(
        """
        CREATE TABLE IF NOT EXISTS applications (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            hr_user_id INTEGER NOT NULL,
            job_seeker_id INTEGER NOT NULL,
            applicant_info_json TEXT NOT NULL,
            total_points INTEGER NOT NULL,
            education_points INTEGER NOT NULL,
            experience_points INTEGER NOT NULL,
            github_points INTEGER NOT NULL,
            portfolio_points INTEGER NOT NULL,
            text_points INTEGER NOT NULL,
            status TEXT NOT NULL,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (hr_user_id) REFERENCES users(id),
            FOREIGN KEY (job_seeker_id) REFERENCES users(id)
        )
        """
    )
    conn.commit()
    conn.close()


def tokenize(text):
    return re.findall(r"[a-zA-Zа-яА-ЯёЁ0-9]{3,}", (text or "").lower())


def train_profile_model():
    train_data = [
        ("мгу факультет вычислительной математики и кибернетики python алгоритмы", "it"),
        ("бауманка программная инженерия разработка c++ python", "it"),
        ("реклама маркетинг digital бренд коммуникации smm", "marketing"),
        ("маркетинговые исследования продуктовый маркетинг контент", "marketing"),
        ("прикладная математика статистика анализ данных sql", "analytics"),
        ("бизнес аналитика data science эконометрика", "analytics"),
        ("логистика управление складом цепи поставок товароведение", "logistics"),
        ("складская логистика транспорт и хранение", "logistics"),
        ("торговое дело кассовое обслуживание клиентский сервис", "retail"),
        ("мерчендайзинг розничная торговля операционный зал", "retail"),
    ]
    label_docs = Counter()
    token_counts = defaultdict(Counter)
    vocab = set()
    for text, label in train_data:
        tokens = tokenize(text)
        label_docs[label] += 1
        for tok in tokens:
            token_counts[label][tok] += 1
            vocab.add(tok)

    return {
        "label_docs": label_docs,
        "token_counts": token_counts,
        "vocab_size": len(vocab),
        "total_docs": sum(label_docs.values()),
    }


def classify_profile(text):
    global PROFILE_MODEL
    if PROFILE_MODEL is None:
        PROFILE_MODEL = train_profile_model()

    tokens = tokenize(text)
    if not tokens:
        return "unknown"

    best_label = "unknown"
    best_score = float("-inf")
    for label, doc_count in PROFILE_MODEL["label_docs"].items():
        prior = math.log(doc_count / PROFILE_MODEL["total_docs"])
        total_label_tokens = sum(PROFILE_MODEL["token_counts"][label].values())
        denom = total_label_tokens + PROFILE_MODEL["vocab_size"]
        score = prior
        for tok in tokens:
            tok_count = PROFILE_MODEL["token_counts"][label].get(tok, 0)
            score += math.log((tok_count + 1) / denom)
        if score > best_score:
            best_score = score
            best_label = label
    return best_label


def get_user_by_id(user_id):
    conn = get_db_connection()
    row = conn.execute("SELECT * FROM users WHERE id = ?", (user_id,)).fetchone()
    conn.close()
    return row



@app.route('/')
def index():
    """Главная страница с двумя кнопками регистрации"""
    return render_template('index.html')



@app.route('/register/hr', methods=['GET', 'POST'])
def register_hr():
    """Форма регистрации HR."""
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']
        access_key = request.form['access_key']

        # Базовая проверка ключа доступа (замените на более надежную систему)
        if access_key != "HR_SECRET_KEY_123":
            return "Неверный ключ доступа. Пожалуйста, свяжитесь с администрацией.", 403

        conn = get_db_connection()
        cur = conn.cursor()
        cur.execute(
            "INSERT INTO users(type, username, password, access_key) VALUES (?, ?, ?, ?)",
            ("hr", username, password, access_key),
        )
        user_id = cur.lastrowid
        conn.commit()
        conn.close()
        return redirect(url_for('hr_profile', user_id=user_id))
    return render_template('register_hr.html')


@app.route('/register/job_seeker', methods=['GET', 'POST'])
def register_job_seeker():
    """Форма регистрации соискателя."""
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']
        conn = get_db_connection()
        cur = conn.cursor()
        cur.execute(
            "INSERT INTO users(type, username, password, access_key) VALUES (?, ?, ?, ?)",
            ("job_seeker", username, password, None),
        )
        user_id = cur.lastrowid
        conn.commit()
        conn.close()
        return redirect(url_for('job_seeker_questionnaire', user_id=user_id))
    return render_template('register_job_seeker.html')


def find_user_by_credentials(user_type, username, password):
    conn = get_db_connection()
    row = conn.execute(
        "SELECT id FROM users WHERE type = ? AND username = ? AND password = ?",
        (user_type, username, password),
    ).fetchone()
    conn.close()
    return row["id"] if row else None


@app.route('/login/hr', methods=['GET', 'POST'])
def login_hr():
    """Вход для HR."""
    error = None
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']

        user_id = find_user_by_credentials('hr', username, password)
        if user_id is None:
            error = "Неверный логин или пароль."
        else:
            return redirect(url_for('hr_profile', user_id=user_id))

    return render_template('login.html', user_type='hr', error=error)


@app.route('/login/job_seeker', methods=['GET', 'POST'])
def login_job_seeker():
    """Вход для соискателя."""
    error = None
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']

        user_id = find_user_by_credentials('job_seeker', username, password)
        if user_id is None:
            error = "Неверный логин или пароль."
        else:
            # После логина соискатель попадает на анкету.
            return redirect(url_for('job_seeker_questionnaire', user_id=user_id))

    return render_template('login.html', user_type='job_seeker', error=error)


def education_score_for_role(target_role, university_name, education_direction, education_completed):
    if not education_completed:
        return 0
    edu_text = f"{university_name} {education_direction}"
    predicted_domain = classify_profile(edu_text)
    role_domain = ROLE_DOMAIN.get(target_role, "unknown")
    return 2 if predicted_domain == role_domain else 1


def text_evaluation(application_format, target_role, experience_description, portfolio_resume_text, personal_qualities, official_experience_years):
    """Простая rule-based оценка текстовой части."""
    role_keywords = {
        "Кассир": ["касса", "продажи", "клиент", "чек", "обслуживание"],
        "Сборщик заказов": ["сбор", "заказ", "склад", "скорость", "внимательность"],
        "Кладовщик": ["склад", "учет", "приемка", "инвентаризация", "логистика"],
        "Python-разработчик": ["python", "api", "django", "flask", "sql", "backend", "git"],
        "Маркетолог": ["реклама", "маркетинг", "smm", "аналитика", "трафик", "кампания"],
        "Аналитик данных": ["sql", "python", "аналитика", "dashboard", "метрика", "данные"],
        "Стажер в IT": ["python", "git", "алгоритм", "проект", "backend", "frontend"],
        "Стажер в маркетинге": ["маркетинг", "контент", "соцсети", "креатив", "бренд"],
        "Стажер в логистике": ["логистика", "склад", "заказ", "поставка", "учет"],
    }
    qualities_keywords = ["команд", "стресс", "ответствен", "коммуника", "инициатив", "дисциплин"]
    text_blob = f"{experience_description} {portfolio_resume_text}".lower()
    matched_keywords = []
    for kw in role_keywords.get(target_role, []):
        if kw in text_blob:
            matched_keywords.append(kw)

    match_flags = []
    if matched_keywords:
        match_flags.append("Совпадение по ключевым словам")

    if official_experience_years > 0:
        has_duration_mention = bool(re.search(r"(\d+)\s*(год|лет|месяц)", experience_description.lower()))
        if has_duration_mention:
            match_flags.append("Описание опыта содержит длительность и согласуется с официальным стажем")

    psychological_signals = [kw for kw in qualities_keywords if kw in personal_qualities.lower()]
    if psychological_signals:
        match_flags.append("Есть данные для базового психологического портрета")

    max_text_points = 4 if application_format == "full_time_prof" else 6
    if application_format in ("internship_prof", "full_time_mass"):
        text_points = min(max_text_points, len(match_flags) * 2)
    else:
        text_points = min(max_text_points, len(match_flags))

    if match_flags:
        summary = (
            f"Краткая текстовая сводка: {', '.join(match_flags)}. "
            f"Подходящие ключевые слова: {', '.join(matched_keywords[:5]) if matched_keywords else 'нет'}."
        )
    else:
        summary = "Краткая текстовая сводка: существенных совпадений по текстовой части не найдено."

    if psychological_signals:
        psych_portrait = (
            "Найдены признаки личностных качеств: "
            + ", ".join(sorted(set(psychological_signals)))
        )
    else:
        psych_portrait = "Личностные качества выражены слабо или не указаны."

    return {
        "text_points": text_points,
        "summary": summary,
        "psych_portrait": psych_portrait,
        "matched_keywords": matched_keywords,
        "match_flags": match_flags,
    }


def score_candidate(form_data):
    official_experience_years = float(form_data.get("official_experience_years", "0") or 0)
    application_format = form_data.get("application_format", "full_time_prof")
    university_name = form_data.get("university_name", "").strip()
    education_direction = form_data.get("education_direction", "").strip()
    education_completed = form_data.get("education_completed", "") == "yes"
    candidate_age = int(form_data.get("candidate_age", "0") or 0)
    age_rejected = candidate_age < 18 or candidate_age > 50

    if application_format == "full_time_mass":
        # Для массовых вакансий не учитываем github/портфолио.
        form_data["github_link"] = ""
        form_data["portfolio_link"] = ""

    education_points = education_score_for_role(
        target_role=form_data.get("target_role", ""),
        university_name=university_name,
        education_direction=education_direction,
        education_completed=education_completed,
    )
    if application_format == "full_time_prof" and not education_completed:
        education_points = -1

    # 1 за полгода = 2 за год; 2 за полгода = 4 за год.
    experience_multiplier = 4 if application_format == "full_time_mass" else 2
    experience_points = int(official_experience_years * experience_multiplier)
    github_points = 1 if form_data.get("github_link", "").strip() else 0
    portfolio_points = 1 if form_data.get("portfolio_link", "").strip() else 0

    text_result = text_evaluation(
        application_format=application_format,
        target_role=form_data.get("target_role", ""),
        experience_description=form_data.get("experience_description", ""),
        portfolio_resume_text=form_data.get("portfolio_resume_text", ""),
        personal_qualities=form_data.get("personal_qualities", ""),
        official_experience_years=official_experience_years,
    )

    total_points = (
        education_points
        + experience_points
        + github_points
        + portfolio_points
        + text_result["text_points"]
    )
    if age_rejected:
        total_points = 0

    return {
        "total_points": total_points,
        "education_points": education_points,
        "experience_points": experience_points,
        "github_points": github_points,
        "portfolio_points": portfolio_points,
        "text_points": text_result["text_points"],
        "text_summary": text_result["summary"],
        "psych_portrait": text_result["psych_portrait"],
        "match_flags": text_result["match_flags"],
        "age_rejected": age_rejected,
    }


@app.route('/questionnaire/job_seeker/<int:user_id>', methods=['GET', 'POST'])
def job_seeker_questionnaire(user_id):
    """Анкета соискателя после логина/регистрации."""
    user = get_user_by_id(user_id)
    if not user or user["type"] != 'job_seeker':
        return "Пользователь не найден или не является соискателем.", 404

    if request.method == 'POST':
        conn = get_db_connection()
        hr_rows = conn.execute("SELECT id FROM users WHERE type = 'hr'").fetchall()
        hr_ids = [row["id"] for row in hr_rows]
        if not hr_ids:
            conn.close()
            return "Пока нет зарегистрированных HR. Попробуйте позже.", 400

        applicant_info = request.form.to_dict()
        if applicant_info.get("personal_data_consent") != "yes":
            conn.close()
            return "Для отправки анкеты требуется согласие на обработку персональных данных.", 400

        applicant_info["job_seeker_id"] = user_id
        applicant_info["username"] = user["username"]

        score_data = score_candidate(applicant_info)
        applicant_info.update(score_data)

        for hr_user_id in hr_ids:
            status = 'Не годен' if score_data["age_rejected"] else 'Новая'
            conn.execute(
                """
                INSERT INTO applications(
                    hr_user_id, job_seeker_id, applicant_info_json, total_points,
                    education_points, experience_points, github_points, portfolio_points,
                    text_points, status
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """,
                (
                    hr_user_id,
                    user_id,
                    json.dumps(applicant_info, ensure_ascii=False),
                    score_data["total_points"],
                    score_data["education_points"],
                    score_data["experience_points"],
                    score_data["github_points"],
                    score_data["portfolio_points"],
                    score_data["text_points"],
                    status,
                ),
            )
        conn.commit()
        conn.close()

        return redirect(url_for('job_seeker_profile', user_id=user_id))

    return render_template(
        'job_seeker_questionnaire.html',
        user_id=user_id,
        application_formats=APPLICATION_FORMATS,
        mass_vacancies=MASS_VACANCIES,
        pro_vacancies=PRO_VACANCIES,
        internships=INTERNSHIP_VACANCIES,
    )


# --- Маршруты профилей ---

@app.route('/profile/hr/<int:user_id>')
def hr_profile(user_id):
    """Страница профиля HR с таблицей заявок."""
    user = get_user_by_id(user_id)
    if not user or user["type"] != 'hr':
        return "Пользователь не найден или не является HR.", 404
    conn = get_db_connection()
    rows = conn.execute(
        "SELECT applicant_info_json, total_points, status FROM applications WHERE hr_user_id = ? ORDER BY total_points DESC, id DESC",
        (user_id,),
    ).fetchall()
    conn.close()
    sorted_applications = []
    for row in rows:
        sorted_applications.append(
            {
                "applicant_info": json.loads(row["applicant_info_json"]),
                "points": row["total_points"],
                "status": row["status"],
            }
        )
    return render_template('hr_profile.html', applications=sorted_applications)


@app.route('/profile/job_seeker/<int:user_id>')
def job_seeker_profile(user_id):
    """Страница профиля соискателя (можно добавить больше полей)."""
    user = get_user_by_id(user_id)
    if not user or user["type"] != 'job_seeker':
        return "Пользователь не найден или не является соискателем.", 404

    conn = get_db_connection()
    row = conn.execute(
        "SELECT applicant_info_json FROM applications WHERE job_seeker_id = ? ORDER BY id DESC LIMIT 1",
        (user_id,),
    ).fetchone()
    conn.close()
    latest_application = json.loads(row["applicant_info_json"]) if row else None

    if not latest_application:
        return (
            f"Добро пожаловать, {user['username']}! "
            f"Анкета еще не заполнена. Перейдите: {url_for('job_seeker_questionnaire', user_id=user_id)}"
        )

    return (
        f"Спасибо, {user['username']}! Ваша анкета отправлена. "
        "Мы свяжемся с вами по указанным контактам при совпадении по требованиям."
    )


# --- Обработка подачи заявок ---

@app.route('/apply/<int:hr_user_id>', methods=['GET', 'POST'])
def apply_for_job(hr_user_id):
    """Маршрут для подачи заявки конкретному HR-пользователю."""
    user = get_user_by_id(hr_user_id)
    if not user or user["type"] != 'hr':
        return "Неверный HR-пользователь.", 404

    if request.method == 'POST':
        applicant_info = request.form.to_dict()  # Собираем все данные из формы

        # Для совместимости старой формы маршрута /apply:
        # маппим поля в новую модель скоринга.
        experience_years = int(applicant_info.get("experience_years", "0") or 0)
        applicant_info.setdefault("application_format", "full_time_prof")
        applicant_info.setdefault("target_role", "Python-разработчик")
        applicant_info.setdefault("university_name", "")
        applicant_info.setdefault("education_direction", applicant_info.get("education_level", ""))
        applicant_info.setdefault("education_completed", "yes")
        applicant_info.setdefault("official_experience_years", str(experience_years))
        applicant_info.setdefault("github_link", "")
        applicant_info.setdefault("portfolio_link", "")
        applicant_info.setdefault("resume_file_link", "")
        applicant_info.setdefault("contact_details", applicant_info.get("email", ""))
        applicant_info.setdefault("candidate_age", "30")
        applicant_info.setdefault("experience_description", applicant_info.get("cover_letter", ""))
        applicant_info.setdefault("portfolio_resume_text", applicant_info.get("cover_letter", ""))
        applicant_info.setdefault("personal_qualities", "")
        applicant_info.setdefault("habit_answer", "")
        applicant_info.setdefault("interest_task_answer", "")
        applicant_info.setdefault("job_seeker_id", 0)
        score_data = score_candidate(applicant_info)
        applicant_info.update(score_data)
        status = 'Не годен' if score_data["age_rejected"] else 'Новая'
        conn = get_db_connection()
        conn.execute(
            """
            INSERT INTO applications(
                hr_user_id, job_seeker_id, applicant_info_json, total_points,
                education_points, experience_points, github_points, portfolio_points,
                text_points, status
            ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """,
            (
                hr_user_id,
                applicant_info["job_seeker_id"],
                json.dumps(applicant_info, ensure_ascii=False),
                score_data["total_points"],
                score_data["education_points"],
                score_data["experience_points"],
                score_data["github_points"],
                score_data["portfolio_points"],
                score_data["text_points"],
                status,
            ),
        )
        conn.commit()
        conn.close()

        return "Заявка успешно отправлена!", 200

    # Отображение формы для подачи заявки (обычно это страница вакансии)
    return render_template('apply_form.html', hr_user_id=hr_user_id)


def main():
    init_db()
    port = int(os.environ.get("PORT", 5000))
    app.run(host='127.0.0.1', port=port)


if __name__ == '__main__':
    main()