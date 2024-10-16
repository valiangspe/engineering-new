#!/usr/bin/python3

from argparse import ArgumentParser
import json
import pprint
import subprocess
import sys

# import yaml

parser = ArgumentParser("Eng manager")

parser.add_argument("action", help="Run: run | build")
parser.add_argument("type", help="Run: backend | frontend | frontend-docker")
parser.add_argument("deploy", help="Type: dev | prod | prod-local")
parser.add_argument("--file_name", help="File name for build: 0.0.2.zip")

args = parser.parse_args()

# Check run type
if (
    args.type != "backend"
    and args.type != "frontend"
    and args.type != "frontend-docker"
):

    print(f"Type invalid: {args.type}")
    parser.print_usage()

    sys.exit(1)

# Check deploy type
if (
    args.deploy != "dev"
    and args.deploy != "dev-docker"
    and args.deploy != "prod"
    and args.deploy != "prod-local"
    and args.deploy != "prod-local2"
    and args.deploy != "prod2"
    and args.deploy != "prod-docker"
    and args.deploy != "prod-intercon"
):

    print(f"Deploye invalid: {args.deploy}")
    parser.print_usage()

    sys.exit(1)

# Backend template file
application_template_properties_file = open("backend.env", "r+")
application_template_properties_contents = application_template_properties_file.read()
application_template_properties_file.close()

# Frontend template file
frontend_template_properties_file = open("frontend.template.env", "r+")
frontend_template_properties_contents = frontend_template_properties_file.read()
frontend_template_properties_file.close()

env_json_file = open("config.json", "r+")
env_json_file_contents = env_json_file.read()
env_json_file.close()

env_json = json.loads(env_json_file_contents)

pprint.pprint(env_json[args.deploy])
pprint.pprint(application_template_properties_contents)

# Replace backend envs
for p in env_json[args.deploy]:
    application_template_properties_contents = (
        application_template_properties_contents.replace(
            f"#{{{p}}}", env_json[args.deploy][p]
        )
    )

print(application_template_properties_contents)

# Replace frontend envs
for p in env_json[args.deploy]:
    frontend_template_properties_contents = (
        frontend_template_properties_contents.replace(
            f"#{{{p}}}", env_json[args.deploy][p]
        )
    )

print(frontend_template_properties_contents)

# Write to application.properties
application_properties_prod_file = open("./backend/.env", "w+")
application_properties_prod_file.write(application_template_properties_contents)
application_properties_prod_file.close()

# Write to frontend
frontend_env_prod_file = open("./frontend/.env", "w+")
frontend_env_prod_file.write(frontend_template_properties_contents)
frontend_env_prod_file.close()

steps = []

if args.type == "frontend":
    if args.action == "run":
        steps = [
            # ('npm i', './frontend'),
            ("yarn dev", "./frontend")
        ]

    elif args.action == "build":
        steps = [
            # ('npm i', './frontend'),
            ("npm run build", "./frontend"),
            ("zip -r release.zip .", "./frontend/build"),
        ]

if args.type == "frontend-docker":
    if args.action == "run":
        steps = [
            # ('npm i', './frontend'),
            ("docker build -t ppicfrontenddev -f ./Dockerfile-frontend .", "."),
            (
                "docker run --rm --name ppicfrontenddev -p 3000:3000 -t ppicfrontenddev",
                ".",
            ),
        ]

elif args.type == "backend":
    if args.action == "run":
        steps = [("dotnet run", "./backend")]
        # steps = [
        #     ('docker build -t ppicdev .', '.'),
        #     ('docker run --rm --name ppicdev -p 8080:8080 ppicdev', '.'),
        # ]
    if args.action == "run-docker":
        steps = [
            ("docker build -t ppicdev .", "."),
            ("docker run --rm --name ppicdev -p 8080:8080 ppicdev", "."),
        ]

    elif args.action == "build":
        # if args.deploy == 'prod-docker':
        steps = [
            (f'docker build -t {env_json[args.deploy]["registry_url"]}/eng .', "."),
            (f'docker push {env_json[args.deploy]["registry_url"]}/eng', "."),
        ]
        # else:
        #     if args.file_name == '' or args.file_name == None:
        #         print('File name (--file_name) must be supplied')
        #         parser.print_usage()

        #         sys.exit(1)

        #     steps = [
        #         ('rm -rf dist', '.'),
        #         ('rm -rf dist src/main/resources/static/*', './backend'),
        #         ('mkdir -p dist', '.'),
        #         # # ('npm i', './frontend'),
        #         # # ('npm run build', './frontend'),
        #         # ('cp -r frontend/build/* backend/src/main/resources/static', '.'),
        #         ('./mvnw clean install spring-boot:repackage', './backend'),
        #         ('cp -r target/ppic-0.0.1-SNAPSHOT.jar ../dist', './backend'),
        #         (f'zip -r {args.file_name} *', './dist')
        #     ]

for step, cwd in steps:
    subprocess.run(step, shell=True, cwd=cwd)
