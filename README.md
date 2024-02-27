# Chirp
Chirp project for 3rd Semester

# Contents

- [How to set up](#how-to-set-up)
  - [How to set up **_Chirp_**](#how-to-set-up-chirp)
    - [Set up Sql Server with Docker](#set-up-sql-server-with-docker)
  - [How to set up tests](#how-to-set-up-tests)
- [How to run **_Chirp_**](#how-to-run-chirp)
- [How to run tests](#how-to-run-tests)
- [How to build report from markdown to pdf](#how-to-build-report-from-markdown-to-pdf)

---

# How to set up
First, clone the repository with the following command if you have SSH keys set up for Github:
```shell
git clone git@github.com:ITU-BDSA23-GROUP11/Chirp.git
```
otherwise, if you don't have SSH keys set up for Github, the following command can be used:
```shell
git clone https://github.com/ITU-BDSA23-GROUP11/Chirp.git
```

Thereafter, in order to set up the project, the main dependency you need is `.NET 7.0`.
It can be downloaded from the from [the _'Download .NET 7.0'_ website](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
> _Make sure to download `.NET 7.0` and not `.NET 8.0`, as **Chirp** will not work otherwise_.

Thereafter, depending on what you want to do, here are the setup guides for the main application, and for tests:
- [How to set up **_Chirp_**](#how-to-set-up-chirp)
- [How to set up tests](#how-to-set-up-tests)

---

# How to run _Chirp_

Ensure that docker is running.

At the root directory run this command:
```shell
DB_PASSWORD=<your_actual_password> docker-compose up
```

Wait for the magic to happen and go to http://localhost:8080/

---

## How to set up tests
Tests have only one requirement, which is needed to run end-to-end tests: playwright.

First of all, the powershell dotnet tool is needed, which can be installed with the following command:
```shell
dotnet tool install PowerShell --version 7.4.0
```

After running the tests the first time, and failing, the cause will be due to playwright not be installed. This can can be solved by running the following command:
```shell
dotnet pwsh \
   test/Chirp.WebService.Tests/bin/Debug/net7.0/playwright.ps1 \
   install
 ```

Everything should now be set up in order to enable tests to run.

---

# How to run tests
To run tests, given the it is set up (see [How to set up tests](#how-to-set-up-tests)), simply run the following command:
```shell
dotnet test --verbosity normal
```

---

# How to build report from markdown to pdf
In order to build the report from the `report.md` file to a `.pdf` file, follow these steps:
1. Go to [the workflow on Github](https://github.com/ITU-BDSA23-GROUP11/Chirp/actions/workflows/report_build.yml)
2. Press the `Run workflow` button/dropdown
3. Select the branch where you made edits in `report.md` (never select main, as the workflow will fail due to branch protection)

A commit will then be added to your branch, containing the built report.