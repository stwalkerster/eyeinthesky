# Stage 1: Build
FROM mono:latest AS build

WORKDIR /src

# Copy source code
COPY ./ ./

# Build the solution in Release mode
RUN nuget restore EyeInTheSky.sln
RUN msbuild EyeInTheSky.sln /p:Configuration=Release /p:Version=2.6.0-k8s

# Stage 2: Runtime
FROM mono:latest

WORKDIR /app

# Copy built assemblies from build stage
COPY --from=build /src/EyeInTheSky/bin/Release/ ./

RUN sed -i -re 's#EyeInTheSky\.Web\.dll#/app/EyeInTheSky.Web.dll#' modules.xml

# Buster is ancient...
RUN sed -i -re 's#//deb\.debian#//archive.debian#g' /etc/apt/sources.list

RUN rm /etc/apt/sources.list.d/mono-official-stable.list
RUN apt-get update && apt-get install -qy net-tools iproute2

RUN rm -f /app/web.xml && ln -sf /conf/web.xml /app/web.xml

RUN adduser --home /app --no-create-home --uid 1000 eyeinthesky

EXPOSE 8080
VOLUME /conf
USER eyeinthesky

ENTRYPOINT ["mono", "EyeInTheSky.exe", "/conf/config.xml"]