﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADProj.Models;
using ADProj.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADProj.Controllers
{
    public class InventoryItemController : Controller
    {
        private InventoryService invService;

        public InventoryItemController(InventoryService invService)
        {
            this.invService = invService;
        }
        public IActionResult Index()
        {
            List<InventoryItem> itemList = invService.ItemList();
            ViewData["itemList"] = itemList;
            if (TempData["alertMsg"] != null)
            {
                ViewData["alertMsg"] = TempData["alertMsg"];
            }
            return View();
        }

        public IActionResult AddItemCategory()
        {
            if (TempData["alertMsg"] != null)
            {
                ViewData["alertMsg"] = TempData["alertMsg"];
            }
            return View();
        }

        public IActionResult SaveItemCategory(string categoryName)
        {
            if (categoryName != null)
            {
                invService.CreateCategory(categoryName);
                TempData["alertMsg"] = "Saved successfully!";
            }
            else
            {
                TempData["alertMsg"] = "Please enter category name!";
            }
            
            return RedirectToAction("AddItemCategory");
        }


        public IActionResult AddInventoryItem()
        {
            List<ItemCategory> catList = invService.CategoryList();
            List<string> catNameList = new List<string>();
            foreach(ItemCategory cat in catList)
            {
                catNameList.Add(cat.Name);
            }

            if (TempData["alertMsg"] != null)
            {
                ViewData["alertMsg"] = TempData["alertMsg"];
            }

            ViewBag.categoryList = catNameList;
            return View();
        }

        public IActionResult SaveInventoryItem(string id, string desc,string catName,string bin,
            string qtyInStock, string reorderLevel,string reorderQty, string uom)
        {
            bool isNum1 = int.TryParse(qtyInStock, out int stockQty);
            bool isNum2 = int.TryParse(reorderLevel, out int reorderLev);
            bool isNum3 = int.TryParse(reorderQty, out int orderQty);
            if (!(id!=null && desc!= null && catName!=null && bin!=null && qtyInStock!=null && reorderLevel != null && reorderQty!=null && uom!=null))
            {
                TempData["alertMsg"] = "Please enter all information!";
                return RedirectToAction("AddInventoryItem");
            }
            else if(!isNum1 || !isNum2 || !isNum3)
            {
                TempData["alertMsg"] = "Quantity must be number!";
                return RedirectToAction("AddInventoryItem");
            }
            else if (stockQty < 0)
            {
                TempData["alertMsg"] = "Quantity in stock must not be negative number!";
                return RedirectToAction("AddInventoryItem");
            }
            else if (reorderLev < 0)
            {
                TempData["alertMsg"] = "Reorder level must not be negative number!";
                return RedirectToAction("AddInventoryItem");
            }
            else if (orderQty < 0)
            {
                TempData["alertMsg"] = "Reorder quantity must not be negative number!";
                return RedirectToAction("AddInventoryItem");
            }
            else
            {
                invService.CreateItem(id, desc, catName, bin, stockQty, reorderLev, orderQty, uom);
                TempData["alertMsg"] = "Saved successfully!";
                return RedirectToAction("AddInventoryItem");
            }            
        }

        public IActionResult CategoryList()
        {
            List<ItemCategory> catList = invService.CategoryList();
            ViewData["CatList"] = catList;
            if (TempData["alertMsg"] != null)
            {
                ViewData["alertMsg"] = TempData["alertMsg"];
            }
            return View();
        }

        public IActionResult EditDeleteCategory(string cmd, int catId)
        {
            ViewData["alertMsg"] = TempData["alertMsg"];

            if (cmd == "delete")
            {
                invService.DeleteCategoryById(catId);
                TempData["alertMsg"] = "Deleted successfully!";
                return RedirectToAction("CategoryList");
            }
            if (cmd == "edit")
            {
                ItemCategory cat=invService.GetCategoryById(catId);
                ViewData["category"] = cat;
                return View("UpdateItemCategory");
            }
            return RedirectToAction("CategoryList");
        }

        public IActionResult UpdateCategory(int id, string categoryName)
        {
            if (categoryName != null)
            {
                invService.UpdateCategoryById(id, categoryName);
                TempData["alertMsg"] = "Updated successfully!";
                return RedirectToAction("CategoryList");
            }
            else
            {
                TempData["alertMsg"] = "Please enter category name!";
                return RedirectToAction("EditDeleteCategory", new { cmd = "edit", catId = id });
            }
            
            
        }

        public IActionResult EditDeleteItem(string cmd, string itemId)
        {
            ViewData["alertMsg"] = TempData["alertMsg"];

            if (cmd == "delete")
            {
                invService.DeleteItemById(itemId);
                TempData["alertMsg"] = "Deleted successfully!";
                return RedirectToAction("Index");
            }
            if (cmd == "edit")
            {
                List<ItemCategory> catList = invService.CategoryList();
                List<string> catNameList = new List<string>();
                foreach (ItemCategory cat in catList)
                {
                    catNameList.Add(cat.Name);
                }
                ViewBag.categoryList = catNameList;

                InventoryItem item = invService.GetItemById(itemId);
                ViewData["item"] = item;
                return View("UpdateInventoryItem");
            }
            if (cmd == "manage")
            {
                InventoryItem item = invService.GetItemById(itemId);
                ViewData["item"] = item;
                return View("ManageInventoryItem");
            }
            return RedirectToAction("Index");
        }

        public IActionResult UpdateInvItem(string id, string desc, string catName, string bin,
            string qtyInStock, string reorderLevel, string reorderQty, string uom)
        {
            bool isNum1 = int.TryParse(qtyInStock, out int stockQty);
            bool isNum2 = int.TryParse(reorderLevel, out int reorderLev);
            bool isNum3 = int.TryParse(reorderQty, out int orderQty);
            if (!(id != null && desc != null && catName != null && bin != null && qtyInStock != null && reorderLevel != null && reorderQty != null && uom != null))
            {
                TempData["alertMsg"] = "Please enter all information!";
                return RedirectToAction("EditDeleteItem", new { cmd = "edit", itemId = id });
            }
            else if (!isNum1 || !isNum2 || !isNum3)
            {
                TempData["alertMsg"] = "Quantity must be number!";
                return RedirectToAction("EditDeleteItem", new { cmd = "edit", itemId = id });
            }
            else if (stockQty < 0)
            {
                TempData["alertMsg"] = "Quantity in stock must not be negative number!";
                return RedirectToAction("EditDeleteItem", new { cmd = "edit", itemId = id });
            }
            else if (reorderLev < 0)
            {
                TempData["alertMsg"] = "Reorder level must not be negative number!";
                return RedirectToAction("EditDeleteItem", new { cmd = "edit", itemId = id });
            }
            else if (orderQty < 0)
            {
                TempData["alertMsg"] = "Reorder quantity must not be negative number!";
                return RedirectToAction("EditDeleteItem", new { cmd = "edit", itemId = id });
            }
            else
            {
                invService.UpdateItemById(id, desc, catName, bin, stockQty, reorderLev, orderQty, uom);
                TempData["alertMsg"] = "updated successfully!";
                return RedirectToAction("Index");
            }
        }

        public IActionResult ManageInvItem(string itemId, string inputupdateQty)
        {
            bool isNum = int.TryParse(inputupdateQty, out int updateQty);

            if (!(inputupdateQty != null))
            {
                TempData["alertMsg"] = "Please enter update quantity!";
                return RedirectToAction("EditDeleteItem",new { cmd="manage",itemId=itemId});
            }
            else if (!isNum)
            {
                TempData["alertMsg"] = "Update quantity must be number!";
                return RedirectToAction("EditDeleteItem", new { cmd = "manage", itemId = itemId });
            }
            else if (updateQty <= 0)
            {
                TempData["alertMsg"] = "Update quantity must be greater than 0!";
                return RedirectToAction("EditDeleteItem", new { cmd = "manage", itemId = itemId });
            }
            else
            {
                int empId = Convert.ToInt32(HttpContext.Session.GetString("id"));
                invService.CreateInvMgmt(itemId, updateQty, empId);
                TempData["alertMsg"] = "Saved successfully!";
                return RedirectToAction("Index");
            }
        }
    }
}