from flask import Flask, render_template, request, redirect, url_for
import  os
app = Flask(__name__)

users = {}

job_applications = {}
application_counter = 1
user_counter = 1



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
        # Редирект на профиль соискателя после успешной регистрации
        return redirect(url_for('job_seeker_profile', user_id=user_id))
    # Отображение формы регистрации соискателя
    return render_template('register_job_seeker.html')


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

    # Заглушка для деталей профиля соискателя
    return f"Добро пожаловать, {users[user_id]['username']}! Это ваша страница профиля."


# --- Обработка подачи заявок ---

@app.route('/apply/<int:hr_user_id>', methods=['GET', 'POST'])
def apply_for_job(hr_user_id):
    """Маршрут для подачи заявки конкретному HR-пользователю."""
    if hr_user_id not in users or users[hr_user_id]['type'] != 'hr':
        return "Неверный HR-пользователь.", 404

    if request.method == 'POST':
        global application_counter
        applicant_info = request.form.to_dict()  # Собираем все данные из формы

        # Базовый расчет очков (замените на вашу логику скоринга)
        points = 0
        if 'experience_years' in applicant_info and applicant_info['experience_years'].isdigit():
            points += int(applicant_info['experience_years']) * 5
        if 'skills' in applicant_info and 'Python' in applicant_info['skills']:
            points += 10
        if 'education_level' in applicant_info and applicant_info[
            'education_level'] == 'Магистратура':  # Пример поля для выбора
            points += 15

        # Если этот HR еще не имеет заявок, создаем для него список
        if hr_user_id not in job_applications:
            job_applications[hr_user_id] = {}

        app_id = application_counter
        job_applications[hr_user_id][app_id] = {
            'applicant_info': applicant_info,
            'points': points,
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