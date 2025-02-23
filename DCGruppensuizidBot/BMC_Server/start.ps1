Set-Location -Path (Split-Path -Path $MyInvocation.MyCommand.Path -Parent)

java -jar .\server.jar nogui
ls