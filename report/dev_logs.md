# Changes made:
## Dockerfile linter hedolint:
when running hedolint this was the output:
```shell
hadolint Dockerfile  
Dockerfile:11 DL4006 warning: Set the SHELL option -o pipefail before RUN with a pipe in it. If you are using /bin/sh in an alpine image or if your shell is symlinked to busybox then consider explicitly setting your SHELL to /bin/ash, or disable this check
Dockerfile:12 DL3008 warning: Pin versions in apt get install. Instead of `apt-get install <package>` use `apt-get install <package>=<version>`
Dockerfile:12 DL3015 info: Avoid additional packages by specifying `--no-install-recommends`
Dockerfile:14 DL3059 info: Multiple consecutive `RUN` instructions. Consider consolidation.
```
the lines where there are warnings was:
```
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs
```

the changes made to meet these warnings was:
```
SHELL ["/bin/bash", "-o", "pipefail", "-c"]
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get update \
    && apt-get install -y --no-install-recommends nodejs=16.x.x-1nodesource1
```

- The first warning ensures that if the commands in the pipe operator fails, the whole build fails.
- The second wants the Node version to be pinned to a specific version.
- The third ensures that no additionally packages are installed with the node installment.
- And the forth and last one says it is more efficient to run the two run commands in one.

After these changes, a new INFO was provided by hedolint:
```shell
Dockerfile:12 DL3009 info: Delete the apt-get lists after installing something
```
Deleting the apt-get list will make the image size smaller, making the last change:
```
SHELL ["/bin/bash", "-o", "pipefail", "-c"]
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get update \
    && apt-get install -y --no-install-recommends nodejs=16.x.x-1nodesource1 \
    && rm -rf /var/lib/apt/lists/*
```