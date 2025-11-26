terraform {
  required_providers {
    vsphere = {
      source  = "hashicorp/vsphere"
      version = ">= 2.5.0"
    }
  }
}

provider "vsphere" {
  user = var.vsphere_user
  allow_unverified_ssl = true
  password = var.vsphere_password
  vsphere_server = var.vsphere_server
}

data "vsphere_datacenter" "dc" {
  name = var.datacenter
}

resource "vsphere_virtual_machine" "windows_vm" {
  memory = 64 * 1024
  num_cpus = 8
  name = var.vm_name
  disk {
    size = 40
    label = "disk0"
  }
}