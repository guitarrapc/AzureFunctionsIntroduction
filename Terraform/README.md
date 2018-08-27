## Run on CloudShell

This terraform aim to run on CloudShell.

[![Launch Cloud Shell](https://shell.azure.com/images/launchcloudshell.png "Launch Cloud Shell")](https://shell.azure.com)

## Prerequisite

Please create any blob storage you want to store your state.
Run following at first time you logged in to CloudShell.

### Public Repo

```bashrc
# load custom bashrc on startup
$ echo "source bashrc" >> .bashrc
$ cat << 'EOF' > bashrc
export ARM_ACCESS_KEY=<....>
export TF_VAR_TENANT_ID=$ACC_TID
export TF_VAR_SP_OBJECT_ID=$ACC_OID
EOF
```

done! Let's try restart cloud shell.

### Private Repo

if you are cloning from private repo, then use ssh auth.

```bashrc
# load custom bashrc on startup
$ echo "source bashrc" >> .bashrc
$ cat << 'EOF' > bashrc
export ARM_ACCESS_KEY=<....>
export TF_VAR_TENANT_ID=$ACC_TID
export TF_VAR_SP_OBJECT_ID=$ACC_OID
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

## Git Clone

Move to clouddrive and clone git repo.

```shell
cd clouddrive
```

* If you are using Public repo, you can clone repo with https.
* If you are using Private repo, you can clone repo with ssh.

```shell
$ git clone git clone https://github.com/guitarrapc/AzureFunctionsIntroduction.git
```

## Terraform Ops


move to repo's terraform directory.

```shell
$ cd AzureFunctionsIntroduction/Terraform/v2
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

