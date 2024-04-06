﻿using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Category_requests
    {
        [Key]
        public int category_request_id { get; set; }
        public string? category_name { get; set; }
        public int requester_id { get; set; }
        public int is_approved { get; set; }
        public bool is_viewed_by_admin { get;set; }
        public bool is_viewed_by_owner { get; set; }
        public bool is_deleted_by_admin { get; set; }
        public bool is_deleted_by_owner { get; set; }
        public string? rejection_reason { get; set; }
        public DateTime created_at { get; set; }
    }
}
