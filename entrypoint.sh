#!/bin/bash
set -e

# Exit if ConnectionStrings__default is not set
if ([ -z "${ConnectionStrings__default}" ]); then
    echo "ConnectionStrings__default is not set. Exiting..."
    exit 1
fi

# Run migrations if AUTO_MIGRATE is set to true
if [ "$AUTO_MIGRATE" = "true" ]; then
    echo "Running migrations..."
    ./migrate-database postgres "${ConnectionStrings__default}" SpecBox.Migrations.dll
fi

# Run the application
dotnet SpecBox.WebApi.dll --urls=http://+:80
