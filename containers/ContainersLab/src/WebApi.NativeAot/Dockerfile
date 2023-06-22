FROM mcr.microsoft.com/dotnet/sdk:8.0.100-preview.4-alpine3.17-amd64 AS build
WORKDIR /app
# these RUN lines below is optimization for current experiment to account for the fact that I have multiple similar configuration
# best practices of Docker building imply that I put all dependencies in single line
RUN apk add clang binutils musl-dev build-base zlib-static
RUN apk add cmake
RUN apk add openssl-dev openssl-libs-static openssl
RUN apk add icu-static icu-dev
COPY WebApi.NativeAot.csproj .
RUN dotnet restore --runtime linux-musl-x64 WebApi.NativeAot.csproj

COPY . .
RUN dotnet publish -c Release -r linux-musl-x64 -o out WebApi.NativeAot.csproj

FROM scratch AS runtime
WORKDIR /app
COPY --from=build /app/out/WebApi.NativeAot /app/
COPY --from=build /etc/ssl/certs/* /etc/ssl/certs/
ENV ASPNETCORE_URLS="http://+"
EXPOSE 80
ENTRYPOINT ["/app/WebApi.NativeAot"]