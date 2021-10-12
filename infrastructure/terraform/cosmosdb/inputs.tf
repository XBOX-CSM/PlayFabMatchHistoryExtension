variable "resource_group" {
  description = "The resource group"
}

variable "prefix" {
  type        = string
  description = "Resource Name prefix (will be applied to all resource names)"
}

variable "tags" {
  type        = map(string)
  default     = {}
  description = "Tags to be added to the resource"
}

variable "container_name" {
  type        = string
  description = "Name of the database container"
}

variable "partition_key" {
  type        = string
  description = "Path of the Partition Key"
}