This is a discord bot with the abillity to start a minecraft server from inside a discord guild and certain games.
There is no build version or docker - build it yourself.
To run this program create a 'token.txt' file with your discord bot token in the root folder.
To enable the minecraft server create the folder MinecraftServer at /data/{guildUid}/MInecraftServer folder that contains either or both (depending on the current OS) script files 'start.ps1' or 'start.sh'.
To make commands work on your guild edit the ./Command/InitCommmands.cs file and replace/add your guild id in the commands bulk register line.
The bot has some more features I'm too lazy to document now :3
Go read the source
