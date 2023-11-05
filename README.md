# GbbConnect

Program to connect inverters (eg: Deye) and program [GbbVictronWeb.gbbsoft.pl.](https://gbbvictronweb.gbbsoft.pl/)

To connect with inverters program uses SolarmanV5 protocol. (Loggers serial numers: 17xxxxxxx, 21xxxxxxx or 40xxxxxxx) (maybe also: 27xxxxxxx, 23xxxxxxx)

To connect with GbbVictronWeb program uses Mqtt server and own protocol (see Wiki).

GbbConnect remarks:
- Data on disk are grouped using number in column "No". So if you want start new inverter with new data then put new No.

## Download

Last version download: [GbbConnect.msi](http://www.gbbsoft.pl/!download/GbbConnect/GbbConnectSetup.msi) [setup.exe](http://www.gbbsoft.pl/!download/GbbConnect/setup.exe)

## Connection to inverter

if first connection failed than program tries to connect every 5 minutes.

Program connects to inverter every 1 minute and downloads statistic.

## Connection to mqtt

If first connection to mqtt failed then program tries to connect every 5 minutes.

Program every minute sends keepalive messave to mqtt. If connected has been lost then every minute program tries to reconect.

## Setup in [GbbVictronWeb](https://gbbvictronweb.gbbsoft.pl/)

Manual how setup GbbConnect with Deye and GbbVictronWeb: [Manual](https://gbbvictronweb.gbbsoft.pl/Manual?Filters.Id=8)

## History

v1.2 - Deya: Disconnect from grid if Price<0 (option)

v1.1 - move configuration files (eg. Parameters.xml) to <MyDocuments>\GbbConnect\ directory

v1.0 - start version

# GbbConnectConsole

Program on console.

## Download

Last version download: [GbbConnectConsole.zip](http://www.gbbsoft.pl/!download/GbbConnect/GbbConnectConsole.zip)

## Parameters

-? - list of parameters

--dont-wait-for-key -  don't wait for key, but just wait forever

# Docker

Program GbbConnectConsole can be run in docker. File Dockerfile is present in root directory.

## Configuration file

You can use GbbConnect program to create (and test) configuration file (My Documents\Gbb Connect\Parameters.xml). Then file can be move as /root/GbbConnect/Parameters.xml on docker container.

## How to run on Docker

- Install docker (or DockerDesktop)
- Clone GbbConnect: Git clone https://github.com/gbbsoft/GbbConnect
- run from GbbConnect directory: docker build .
- create container: docker container create -i -t --name gbbconnect <your image ID>
- copy Parameters.xml: docker cp ./Parameters.xml gbbconnect:/root/GbbConnect/Parameters.xml
- start container: docker start gbbconnect
- make container always running: docker update --restart unless-stopped gbbconnect

## Sample Parameters.xml file

```
<?xml version="1.0" encoding="utf-8"?>
<Parameters Version="1" GbbVictronWeb_Mqtt_Address="127.0.0.1" GbbVictronWeb_Mqtt_Port="8883" Server_AutoStart="0" IsVerboseLog="1" IsDriverLog="0" IsDriverLog2="0">
  <Plant Version="1" Number="1" Name="MyPlant" InverterNumber="0" IsDisabled="0" PortNo="8899" PriceLess0_DisconnectGrid="1" GbbVictronWeb_UserEmail="<you email>" GbbVictronWeb_PlantId="<you PlantId>" GbbVictronWeb_PlantToken="<Your Token>" />
</Parameters>
```
