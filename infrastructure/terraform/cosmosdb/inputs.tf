variable "resource_group" {
  description = "The resource group"
}

variable "prefix" {
  type        = string
  description = "Resource Name prefix (will be applied to all resource names)"
}

variable "tags" {
  type    = map(string)
  default = {}
}
