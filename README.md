# Advent of Code 2024 Challenge

## Requirements
All requirements are taken from the two-part challenges issued daily on the [Advent of Code](https://adventofcode.com/) website. This specific repo will contain the  code used to solve the problems provided in the 2024 challenge.

Additional constraints added by myself are as follows: 
1. Code used to solve the problems **shall** be written using C#
1. Code **shall** use the current LTS version of dotnet (*net8.0*)
1. Code written shall be entirely my own and no use of AI assistants will be permitted (eg. no use of GitHub Copilot etc.)
1. A **single application** shall be used to solve all daily challenges
1. Data for each daily challenge shall be located in the root folder `data` of the repository
1. Data for each daily challenge shall be supplied in the format day*nn*-input.txt 
1. The application shall hold all code for the daily challenges in separate classes
1. Running of the application:
    1. Shall determine the current list of solutions contained in the application
    1. without parameters: shall execute the solution to the latest problem (both parts)
    1. with `all` as a parameter: shall execute each solution in ascending day order
    1. with a numeric parameter (or parameters): shall execute the solution(s) to the days specified
