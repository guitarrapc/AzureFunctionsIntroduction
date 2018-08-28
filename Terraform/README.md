## TL;DR

This terraform aim to run on both CloudShell and CI.

Environment | Description | Use case
---- | ---- | ----
CloudShell | Managed Console with MSI. | Each developer's cloud dev env.
Local | az login credential | Each developer's local dev env.
CI | Service Principal Id of application registration, terraform. | CI or any non-interactive env

## Notice

* terraform version : Will depends on what CloudShell supports. -> see terraform.tf
* Env : ARM_ACCESS_KEY is required for terraform's backend blob authentication.
* Env : ARM_USE_MSI will redirect your credential with local SP and CloudShell's MSI.

## Run on Local (already az login)

> SAMPLE : bashrc_local_azlogin

```bashrc
export ARM_ACCESS_KEY=<....>
export TF_VAR_SP_OBJECT_ID=<....>
export TF_VAR_FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL=<....>
export TF_VAR_FUNCTION_APP_SLACKINCOMINGWEBHOOKURL=<....>
```


## Run on CI (service principal id for Terraform)

> SAMPLE : bashrc_serviceprincipal

```bashrc
export ARM_ACCESS_KEY=<....>
export ARM_SUBSCRIPTION_ID=<....>
export ARM_CLIENT_ID=<....>
export ARM_CLIENT_SECRET=<....>
export ARM_TENANT_ID=<....>
export TF_VAR_SP_OBJECT_ID=<....>
export TF_VAR_FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL=<....>
export TF_VAR_FUNCTION_APP_SLACKINCOMINGWEBHOOKURL=<....>
```

## Run on CloudShell


[![Launch Cloud Shell](https://shell.azure.com/images/launchcloudshell.png "Launch Cloud Shell")](https://shell.azure.com)

### Prerequisite for CloudShell

Please create any blob storage you want to store your state.
Run following at first time you logged in to CloudShell.

#### Public Repo

> SAMPLE : bashrc_cloudshell_public

```bashrc
# load custom bashrc on startup
$ echo "source bashrc" >> .bashrc
$ cat << 'EOF' > bashrc
export ARM_ACCESS_KEY=<....>
export TF_VAR_SP_OBJECT_ID=<....>
export TF_VAR_FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL=<....>
export TF_VAR_FUNCTION_APP_SLACKINCOMINGWEBHOOKURL=<....>
EOF
```

done! Let's try restart cloud shell.

#### Private Repo

if you are cloning from private repo, then use ssh auth.

> SAMPLE : bashrc_cloudshell_private

```bashrc
# load custom bashrc on startup
$ echo "source bashrc" >> .bashrc
$ cat << 'EOF' > bashrc
export ARM_ACCESS_KEY=<....>
export TF_VAR_SP_OBJECT_ID=<....>
export TF_VAR_FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL=<....>
export TF_VAR_FUNCTION_APP_SLACKINCOMINGWEBHOOKURL=<....>
# ssh agent (for private repo only)
eval $(ssh-agent -s)
ssh-add .ssh/id_rsa
ssh-add -l
EOF
```

Github Authentication with Generate SSH key.

> [Generating a new SSH key and adding it to the ssh-agent](https://help.github.com/articles/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent/
)

```shell
$ ssh-keygen -t rsa -b 4096 -C "YOUR_USER_NAME@users.noreply.github.com"
```

Copy ssh pub and paste to Github Public Key.

> https://github.com/YOUR_ACCOUNT/REPOSITORY/settings/keys

```bash
$ cat ~/.ssh/id_rsa.pub
```

done! Let's try restart cloud shell.
You will find ssh-agen loaded with specified ssh key.

### Git Clone

Move to clouddrive and clone git repo.

```shell
cd clouddrive
```

* If you are using Public repo, you clone repo with https.
* If you are using Private repo, you clone repo with ssh.

```shell
$ git clone git clone https://github.com/guitarrapc/AzureFunctionsIntroduction.git
```

You may required git config for git commit/push ops.

```
$ git config --global user.name "John Doe"
$ git config --global user.email johndoe@users.noreply.github.com
```

## Terraform Ops


move to repo's terraform directory.

```shell
$ cd ~/clouddrive/AzureFunctionsIntroduction/Terraform/v2
```

### tfstate Blob settings

If you chaned your blob, modify `terraform.tf` the remote state file.

```shell
$ cat terraform.tf

terraform {
  required_version = "> 0.11.0"

  backend "azurerm" {
    storage_account_name = "terraformguitarrapc"
    container_name       = "azurefunctionsintroduction"
    key                  = "azure/v2/terraform.tfstate"
    resource_group_name  = "terraform"
  }
}
```

### Terraform init

Operate with terraform init, plan, apply!

```shell
$ terraform init

Initializing the backend...

Successfully configured the backend "azurerm"! Terraform will automatically
use this backend unless the backend configuration changes.

Initializing provider plugins...
- Checking for available provider plugins on https://releases.hashicorp.com...
- Downloading plugin for provider "azurerm" (1.13.0)...

Terraform has been successfully initialized!

You may now begin working with Terraform. Try running "terraform plan" to see
any changes that are required for your infrastructure. All Terraform commands
should now work.

If you ever set or change modules or backend configuration for Terraform,
rerun this command to reinitialize your working directory. If you forget, other
commands will detect it and remind you to do so if necessary.
```


### Terraform plan and Apply

```shell
$ terraform plan
$ terraform apply
```