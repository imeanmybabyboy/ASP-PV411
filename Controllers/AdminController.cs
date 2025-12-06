using ASP_PV411.Data;
using ASP_PV411.Models.Admin;
using ASP_PV411.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASP_PV411.Controllers
{
    public class AdminController(DataContext dataContext, IStorageService storageService) : Controller
    {
        public IActionResult Index()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            string? roleId = isAuthenticated
                ? HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                : null;
            if (roleId != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            AdminIndexViewModel model = new()
            {
                Manufacturers = dataContext.Manufacturers.ToList(),
                Groups = [.. dataContext.Groups]
            };

            return View(model);
        }

        public JsonResult AddManufacturer(AdminManufacturerFormModel formModel)
        {
            string? savedName = null!;
            
            if (!(formModel.Image == null))
            {
                try
                {
                    savedName = storageService.Save(formModel.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("admin-manufacturer-image", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("admin-manufacturer-image", "Необхідно додати логотип виробника");
            }

            if (string.IsNullOrEmpty(formModel.Name))
            {
                ModelState.AddModelError("admin-manufacturer-name", "Необхідно вказати назву виробника");
            }
            
            if (string.IsNullOrEmpty(formModel.Description))
            {
                ModelState.AddModelError("admin-manufacturer-description", "Необхідно вказати опис виробника");
            }

            if (ModelState.IsValid)
            {
                Data.Entities.Manufacturer item = new()
                {
                    Id = Guid.NewGuid(),
                    Name = formModel.Name,
                    Descirption = formModel.Description,
                    ImageUrl = savedName
                };
                dataContext.Manufacturers.Add(item);
                dataContext.SaveChanges();
                return Json(new
                {
                    status = "Ok",
                    data = new
                    {
                        formModel.Name,
                        formModel.Description,
                        savedName
                    }
                });
            }
            else
            {
                Dictionary<string, string> errors = [];
                foreach (var kv in ModelState)
                {
                    string errorMessage = string.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errors[kv.Key] = errorMessage;
                    }
                }

                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }

        }

        public JsonResult AddGroup(AdminGroupFormModel formModel)
        {
            string? savedName = null!;

            if (!(formModel.Image == null))
            {
                try
                {
                    savedName = storageService.Save(formModel.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("admin-group-image", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("admin-group-image", "Необхідно додати логотип товарної групи");
            }

            if (string.IsNullOrEmpty(formModel.Name))
            {
                ModelState.AddModelError("admin-group-name", "Необхідно вказати назву товарної групи");
            }

            if (string.IsNullOrEmpty(formModel.Description))
            {
                ModelState.AddModelError("admin-group-description", "Необхідно вказати опис товарної групи");
            }

            if (ModelState.IsValid)
            {
                Data.Entities.Group item = new()
                {
                    Id = Guid.NewGuid(),
                    Name = formModel.Name,
                    Descirption = formModel.Description,
                    ImageUrl = savedName
                };
                dataContext.Groups.Add(item);
                dataContext.SaveChanges();
                return Json(new
                {
                    status = "Ok",
                    data = new
                    {
                        formModel.Name,
                        formModel.Description,
                        savedName
                    }
                });
            }
            else
            {
                Dictionary<string, string> errors = [];
                foreach (var kv in ModelState)
                {
                    string errorMessage = string.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errors[kv.Key] = errorMessage;
                    }
                }

                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }

        }

        public JsonResult AddProduct(AdminProductFormModel formModel)
        {
            string? savedName = null!;
            Guid groupId = Guid.Empty;
            Guid manufacturerId = Guid.Empty;

            // валідація виробника
            try
            {
                manufacturerId = Guid.Parse(formModel.ManufacturerId);
            }
            catch (Exception)
            {
                ModelState.AddModelError("admin-product-manufacturer", "Невідомий виробник");
            }

            // валідація товарної групи
            try
            {
                groupId = Guid.Parse(formModel.GroupId);
            }
            catch (Exception)
            {
                ModelState.AddModelError("admin-product-group", "Невідома товарна група");
            }

            // Валідація картинки
            if (!(formModel.Image == null))
            {
                try
                {
                    savedName = storageService.Save(formModel.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("admin-product-image", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("admin-product-image", "Необхідно додати картинку до товару");
            }

            // Валідація назви
            if (string.IsNullOrEmpty(formModel.Name))
            {
                ModelState.AddModelError("admin-product-name", "Необхідно вказати назву товару");
            }

            // Валідація опису
            if (string.IsNullOrEmpty(formModel.Description))
            {
                ModelState.AddModelError("admin-product-description", "Необхідно вказати опис товару");
            }

            if (ModelState.IsValid)
            {
                if (formModel.Image != null && formModel.Image.Length > 0)
                {
                    try
                    {
                        savedName = storageService.Save(formModel.Image);
                    }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            status = "Error",
                            errors = new Dictionary<string, string>
                        {
                            { "admin-product-image", ex.Message }
                        },
                        });
                    }
                }

                Data.Entities.Product item = new()
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    ManufacturerId = manufacturerId,
                    Name = formModel.Name,
                    Descirption = formModel.Description,
                    ImageUrl = savedName,
                    Price = formModel.Price,
                    Stock = formModel.Stock,
                };
                dataContext.Products.Add(item);
                dataContext.SaveChanges();
                return Json(new
                {
                    status = "Ok",
                    data = item
                });
            }
            else
            {
                Dictionary<string, string> errors = [];
                foreach (var kv in ModelState)
                {
                    string errorMessage = string.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errors[kv.Key] = errorMessage;
                    }
                }

                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }

        }
    }
}
