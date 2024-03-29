# Logging and Tracing demo

This repository contains all the code for the demo used in the presentation at [DevCampNoord 2019](https://devnetnoord.github.io/).

The `master` branch contains the end result, if you want to walk through the steps you can check out the individual branches from `steps/step-0` 
which is the Firehose example to `steps/step-9` that is the completed solution.

To quickly experiment with a particular step you can run the following command from a PowerShell console in the `demo` folder:

`docker-compose rm -f; .\build.ps1 -Target Build-Release --ScriptArgs "--BuildVersion=0.<number of step>"; docker-compose up`

this will remove running containers, rebuild the solution and containers, start up the newly built containers.

> **NOTE** replace `<number of step>` with the number of the step (duh) because otherwise docker compose won't start as the container tag won't match.

To see the results you can open your browser and point it at [http://localhost:8080/](http://localhost:8080) to use Seq.