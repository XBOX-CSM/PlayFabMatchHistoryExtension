variable "location" {
  type        = string
  description = "The location in which to create all resources"
  default     = "westeurope"
}

variable "prefix" {
  type        = string
  description = "Resource Name prefix (will be applied to all resource names, including the resource group"
  default     = "pfmatchhistory"
}

variable "tags" {
  type    = map(string)
  default = {}
}

variable "pf_title_id" {
  type        = string
  description = "The PlayFab Title ID to authenticate against"
}

variable "pf_developer_secret" {
  type        = string
  description = "The PlayFab developer secret for the Title"
}