# GameLauncher

> The **GameLauncher** app was initially ordered by a company who wanted to let potential customers/gamers play short demos of games from some limited collection before they could buy them and record video of their reactions and behaviour for later study.

Main features are:
- Administrators should register first. The app stores their logins and encrypted passwords in Windows Registry
- Information regarding games and logs is stored in an SQLite database file
- Administrators can add, edit and remove games
- After a user launches selected game, the app starts recording video from a webcam (if it's available) of him/her playing the game. After some short trial period of time (specified by an administrator for each game) the app interrupts the process and proposes to buy the full version of the game. Info about each launch of a game is also recorded to the log.
- Administrators can view logs, filter log data by games and their launch dates

*Just a "humble" note about the MVVM-ness of code: check out those empty view's code-behinds and bindable view helpers! :-)*

Requirements:
* Windows OS
* .NET Framework 3.5 or higher

Main window

![pic1](https://github.com/ar1st0crat/GameLauncher/blob/master/Screenshots/1.png)

Log & stats

![pic2](https://github.com/ar1st0crat/GameLauncher/blob/master/Screenshots/2.png)

PS. The game posters in the screenshots were replaced with covers of various albums by the prog-rock band Camel. I hope, Andy Latimer and co wouldn't mind :-)