# Команди Бота

- **/start** — Підписатися на отримання квитків
- **/unsubscribe** — Відписатися
- **/check** — Перевірити, чи бот успішно працює


## Обовязково !!!
1. Вставте Id свого бота в - **botId** поле в файлі appsetting.json
2. Пишемо /start в боті.
3. При першому та деяких подальших запусках вам потрібно оновити креди.
- **/cookie param** — Встановити cookie
- **/rsftoken param** — Встановити X-RSF-Token
#### Заходимо в свій акаунт в СЦ і аналізуємо запит та витягуємо потрібну інфу як на фото. Тобто тикаємо на здачу на права, наприклад b1, мезаніка і там будуть йти запити. Потім закидаємо з командами які зазначенні вище.
![Приклад](https://github.com/arsenpaw/TicketParse/blob/master/example1.png)
3. Впринципі все, в меню там лекго розібратись.


## Налаштування додатка, опціонально
- **botId** — Ідентифікатор від BotFather
- **Credential** — Заголовки з запиту з мапи у тесті практики (натисніть на сіре коло та проаналізуйте запит)
- **StartSearchingDate** — Дата початку пошуку (наприклад, "2025-02-18"). Можна не рухати або залишити пустим, тоді буде братись сьогоднішня дата.
  - **DaysToSearch** — Кількість днів від початкової дати, протягом яких бот шукатиме квиток (наприклад, 14)
- **OfficeId** — Ну рухати.
- **MinutesRequestDelay** — Затримка між запитами. Наприклад, якщо значення 0.5, пошук виконуватиметься кожні 30 секунд. Можна так і залишити
