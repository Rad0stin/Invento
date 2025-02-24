# Invento
Invento е система за управление на инвентар за персонални компютри, разработена на C#. Тя предоставя лесен и ефективен начин за проследяване и управление на продукти, категории и потребители.

Функционалности

Аутентикация: Сигурна система за вход и регистрация на потребители.

Информационно табло: Табло за администратор да вижда промените в неговата система.

Категории: Организиране на продуктите в различни категории за по-лесно навигиране.

Потребители: Управление на потребителски акаунти с различни нива на достъп.

Управление на продукти: Добавяне, редактиране и изтриване на продукти с подробна информация.

Управление на клиенти: Управление на клиенти и следене на промените.

Поръчки: Добавяне на поръяки с касов бон - само за акунт тип "касиер"

Настройки: Персонализиране на приложението според нуждите на потребителя.


Инсталация

Начин 1:

Клониране на репозиторията:

git clone https://github.com/Rad0stin/Invento.git

Отваряне на проекта:

Отворете Invento.sln файла с Visual Studio.

Възстановяване на пакетите:

Visual Studio автоматично ще възстанови необходимите NuGet пакети. Ако не, изберете Restore NuGet Packages от менюто.

Стартиране на проекта:

Натиснете F5 или изберете Start от менюто, за да стартирате приложението.

Изисквания
.NET Framework 4.7.2 или по-нова версия.
Visual Studio 2019 или по-нова версия.
SQL Server за базата данни.
Структура на проекта

Main: Главната форма на приложението.

Products: Форми и класове, свързани с управлението на продукти.

Categories: Форми и класове за управление на категории.

Users: Форми и класове, свързани с потребителската аутентикация и управление.

Settings-Form: Форма за настройките на приложението.

Auth: Форми, свързани с вход и регистрация.

Assets: Ресурси като изображения и икони.

Начин 2:

Начин две е ако искате само да изтеглите и разгледате приложението без да може да правите промени по него.
Изтеглете InventoSetup.exe от github ili ot уебсайта на приложението https://inventosys.netlify.app/, след което ще се отвори инсталационният съветник. Прочетете и приемете лицензионното споразумение, за да продължите. След това ще бъдете подканени да изберете директория за инсталация – по подразбиране програмата ще се инсталира в C:\Program Files..., но можете да изберете друга локация, ако желаете. След успешната инсталация отворете приложението, което носи името Invento. 

Принос
Ако желаете да допринесете към развитието на Invento:

Направете форк на репозиторията.

Създайте нов клон: git checkout -b feature-branch-name.

Направете вашите промени и комитнете: git commit -m 'Add some feature'.

Пушнете промените: git push origin feature-branch-name.

Създайте Pull Request в оригиналния репозитория.
