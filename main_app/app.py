from flask import Flask, render_template, request, redirect, url_for
import os
import re

app = Flask(__name__)

users = {}

job_applications = {}
application_counter = 1
user_counter = 1

MASS_VACANCIES = ["Кассир", "Сборщик заказов", "Кладовщик"]
PRO_VACANCIES = ["Python-разработчик", "Маркетолог", "Аналитик данных"]
INTERNSHIPS = ["Стажер в IT", "Стажер в маркетинге", "Стажер в логистике"]



@app.route('/')
def index():
    """Главная страница с двумя кнопками регистрации"""
    return render_template('index.html')



@app.route('/register/hr', methods=['GET', 'POST'])
def register_hr():
    """Форма регистрации HR."""
    if request.method == 'POST':
        global user_counter
        username = request.form['username']
        password = request.form['password']  # В продакшене используйте безопасное хеширование паролей
        access_key = request.form['access_key']

        # Базовая проверка ключа доступа (замените на более надежную систему)
        if access_key != "HR_SECRET_KEY_123":
            return "Неверный ключ доступа. Пожалуйста, свяжитесь с администрацией.", 403

        user_id = user_counter
        users[user_id] = {'type': 'hr', 'username': username, 'password': password, 'access_key': access_key}
        user_counter += 1
        # Редирект на профиль HR после успешной регистрации
        return redirect(url_for('hr_profile', user_id=user_id))
    # Отображение формы регистрации HR
    return render_template('register_hr.html')


@app.route('/register/job_seeker', methods=['GET', 'POST'])
def register_job_seeker():
    """Форма регистрации соискателя."""
    if request.method == 'POST':
        global user_counter
        username = request.form['username']
        password = request.form['password']  # В продакшене используйте безопасное хеширование паролей

        user_id = user_counter
        users[user_id] = {'type': 'job_seeker', 'username': username, 'password': password}
        user_counter += 1
        # После регистрации сразу переводим на анкету кандидата.
        return redirect(url_for('job_seeker_questionnaire', user_id=user_id))
    # Отображение формы регистрации соискателя
    return render_template('register_job_seeker.html')


def find_user_by_credentials(user_type, username, password):
    """Ищет пользователя по типу, логину и паролю."""
    for user_id, user_data in users.items():
        if (
            user_data.get('type') == user_type
            and user_data.get('username') == username
            and user_data.get('password') == password
        ):
            return user_id
    return None


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


def is_it_education(education_specialization):
    return education_specialization == "it"


def ai_text_evaluation(application_type, target_role, experience_description, portfolio_resume_text, personal_qualities, official_work_months):
    """Простая rule-based имитация AI оценки текстовой части."""
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

    if official_work_months > 0:
        has_duration_mention = bool(re.search(r"(\d+)\s*(год|лет|месяц)", experience_description.lower()))
        if has_duration_mention:
            match_flags.append("Описание опыта содержит длительность и согласуется с официальным стажем")

    psychological_signals = [kw for kw in qualities_keywords if kw in personal_qualities.lower()]
    if psychological_signals:
        match_flags.append("Есть данные для базового психологического портрета")

    max_ai_points = 4 if application_type == "full_time" else 6
    # 0..4 для полной занятости и 0..6 для стажировок.
    if application_type == "full_time":
        ai_points = min(max_ai_points, len(match_flags))
    else:
        ai_points = min(max_ai_points, len(match_flags) * 2)

    if match_flags:
        summary = (
            f"Краткая AI-сводка: {', '.join(match_flags)}. "
            f"Подходящие ключевые слова: {', '.join(matched_keywords[:5]) if matched_keywords else 'нет'}."
        )
    else:
        summary = "Краткая AI-сводка: существенных совпадений по текстовой части не найдено."

    if psychological_signals:
        psych_portrait = (
            "Найдены признаки личностных качеств: "
            + ", ".join(sorted(set(psychological_signals)))
        )
    else:
        psych_portrait = "Личностные качества выражены слабо или не указаны."

    return {
        "ai_points": ai_points,
        "summary": summary,
        "psych_portrait": psych_portrait,
        "matched_keywords": matched_keywords,
        "match_flags": match_flags,
    }


def score_candidate(form_data):
    official_work_months = int(form_data.get("official_work_months", "0") or 0)
    application_type = form_data.get("application_type", "full_time")
    education_specialization = form_data.get("education_specialization", "other")

    education_points = 2 if is_it_education(education_specialization) else 1
    experience_points = official_work_months // 6  # 1 балл за каждые полгода.
    github_points = 1 if form_data.get("github_link", "").strip() else 0
    portfolio_points = 1 if form_data.get("portfolio_link", "").strip() else 0

    ai_result = ai_text_evaluation(
        application_type=application_type,
        target_role=form_data.get("target_role", ""),
        experience_description=form_data.get("experience_description", ""),
        portfolio_resume_text=form_data.get("portfolio_resume_text", ""),
        personal_qualities=form_data.get("personal_qualities", ""),
        official_work_months=official_work_months,
    )

    total_points = (
        education_points
        + experience_points
        + github_points
        + portfolio_points
        + ai_result["ai_points"]
    )

    return {
        "total_points": total_points,
        "education_points": education_points,
        "experience_points": experience_points,
        "github_points": github_points,
        "portfolio_points": portfolio_points,
        "ai_points": ai_result["ai_points"],
        "ai_summary": ai_result["summary"],
        "psych_portrait": ai_result["psych_portrait"],
        "match_flags": ai_result["match_flags"],
    }


@app.route('/questionnaire/job_seeker/<int:user_id>', methods=['GET', 'POST'])
def job_seeker_questionnaire(user_id):
    """Анкета соискателя после логина/регистрации."""
    if user_id not in users or users[user_id]['type'] != 'job_seeker':
        return "Пользователь не найден или не является соискателем.", 404

    if request.method == 'POST':
        global application_counter

        # Публикуем заявку в общий поток всех HR.
        hr_ids = [uid for uid, udata in users.items() if udata.get("type") == "hr"]
        if not hr_ids:
            return "Пока нет зарегистрированных HR. Попробуйте позже.", 400

        applicant_info = request.form.to_dict()
        applicant_info["job_seeker_id"] = user_id
        applicant_info["username"] = users[user_id]["username"]

        score_data = score_candidate(applicant_info)
        applicant_info.update(score_data)

        for hr_user_id in hr_ids:
            if hr_user_id not in job_applications:
                job_applications[hr_user_id] = {}

            app_id = application_counter
            job_applications[hr_user_id][app_id] = {
                'applicant_info': applicant_info.copy(),
                'points': score_data["total_points"],
                'status': 'Новая'
            }
            application_counter += 1

        return redirect(url_for('job_seeker_profile', user_id=user_id))

    return render_template(
        'job_seeker_questionnaire.html',
        user_id=user_id,
        mass_vacancies=MASS_VACANCIES,
        pro_vacancies=PRO_VACANCIES,
        internships=INTERNSHIPS,
    )


# --- Маршруты профилей ---

@app.route('/profile/hr/<int:user_id>')
def hr_profile(user_id):
    """Страница профиля HR с таблицей заявок."""
    if user_id not in users or users[user_id]['type'] != 'hr':
        return "Пользователь не найден или не является HR.", 404

    # Получаем заявки для данного HR, или пустой словарь, если их нет
    hr_applications = job_applications.get(user_id, {})
    # Сортируем заявки по очкам в порядке убывания
    sorted_applications = sorted(hr_applications.values(), key=lambda x: x['points'], reverse=True)

    # Отображение профиля HR с отсортированными заявками
    return render_template('hr_profile.html', applications=sorted_applications)


@app.route('/profile/job_seeker/<int:user_id>')
def job_seeker_profile(user_id):
    """Страница профиля соискателя (можно добавить больше полей)."""
    if user_id not in users or users[user_id]['type'] != 'job_seeker':
        return "Пользователь не найден или не является соискателем.", 404

    latest_application = None
    for hr_data in job_applications.values():
        for app_data in hr_data.values():
            if app_data["applicant_info"].get("job_seeker_id") == user_id:
                latest_application = app_data["applicant_info"]

    if not latest_application:
        return (
            f"Добро пожаловать, {users[user_id]['username']}! "
            f"Анкета еще не заполнена. Перейдите: {url_for('job_seeker_questionnaire', user_id=user_id)}"
        )

    return (
        f"Спасибо, {users[user_id]['username']}! Ваша анкета отправлена. "
        f"Общий балл: {latest_application.get('total_points', 0)}. "
        f"{latest_application.get('ai_summary', '')}"
    )


# --- Обработка подачи заявок ---

@app.route('/apply/<int:hr_user_id>', methods=['GET', 'POST'])
def apply_for_job(hr_user_id):
    """Маршрут для подачи заявки конкретному HR-пользователю."""
    if hr_user_id not in users or users[hr_user_id]['type'] != 'hr':
        return "Неверный HR-пользователь.", 404

    if request.method == 'POST':
        global application_counter
        applicant_info = request.form.to_dict()  # Собираем все данные из формы

        # Для совместимости старой формы маршрута /apply:
        # маппим поля в новую модель скоринга.
        experience_years = int(applicant_info.get("experience_years", "0") or 0)
        applicant_info.setdefault("application_type", "full_time")
        applicant_info.setdefault("target_role", "Python-разработчик")
        applicant_info.setdefault("education_specialization", "it" if "IT" in applicant_info.get("education_level", "").upper() else "other")
        applicant_info.setdefault("official_work_months", str(experience_years * 12))
        applicant_info.setdefault("github_link", "")
        applicant_info.setdefault("portfolio_link", "")
        applicant_info.setdefault("experience_description", applicant_info.get("cover_letter", ""))
        applicant_info.setdefault("portfolio_resume_text", applicant_info.get("cover_letter", ""))
        applicant_info.setdefault("personal_qualities", "")
        score_data = score_candidate(applicant_info)
        applicant_info.update(score_data)

        # Если этот HR еще не имеет заявок, создаем для него список
        if hr_user_id not in job_applications:
            job_applications[hr_user_id] = {}

        app_id = application_counter
        job_applications[hr_user_id][app_id] = {
            'applicant_info': applicant_info,
            'points': score_data["total_points"],
            'status': 'Новая'  # Начальный статус заявки
        }
        application_counter += 1

        return "Заявка успешно отправлена!", 200

    # Отображение формы для подачи заявки (обычно это страница вакансии)
    return render_template('apply_form.html', hr_user_id=hr_user_id)


def main():
    port = int(os.environ.get("PORT", 5000))
    app.run(host='127.0.0.1', port=port)


if __name__ == '__main__':
    main()