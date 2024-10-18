using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EwayBill.Models;
using EwayBill.Services;

namespace EwayBill.Controllers
{
    public class MastersController : Controller
    {
        private readonly MastersService _mastersService = new MastersService();

        #region Manage Role
        [HttpPost]
        public async Task<ActionResult> SaveManageRole(ManageRole manageRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _mastersService.SaveManageRole(manageRole);
                    return Redirect("/Admin/dist/pages/Masters/ManageRole.html");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(manageRole);
        }

        [HttpGet]
        public async Task<JsonResult> GetManageRole(int? page = null, int? pageSize = null, string searchQuery = null)
        {
            try
            {
                List<ManageRole> roles;
                if (page.HasValue && pageSize.HasValue)
                {
                    roles = await _mastersService.GetManageRole(0, page.Value, pageSize.Value, searchQuery);
                }
                else
                {
                    roles = await _mastersService.GetManageRole(0, null, null, searchQuery);
                }

                int totalItems = 0;
                if (page.HasValue && pageSize.HasValue)
                {
                    totalItems = await _mastersService.GetTotalRoleCount(searchQuery);
                }

                return Json(new { roles, totalItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckManageRole(string role)
        {
            bool exists = _mastersService.IsManageRoleTaken(role);
            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteManageRole(int roleID)
        {
            try
            {
                string message = await _mastersService.DeleteManageRole(roleID);
                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Manage State
        [HttpPost]
        public async Task<ActionResult> SaveManageState(ManageState manageState)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _mastersService.SaveManageState(manageState);
                    return Redirect("/Admin/dist/pages/Masters/ManageState.html");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(manageState);
        }

        [HttpGet]
        public async Task<JsonResult> GetManageState(int? page = null, int? pageSize = null, string searchQuery = null)
        {
            try
            {
                List<ManageState> states;
                if (page.HasValue && pageSize.HasValue)
                {
                    states = await _mastersService.GetManageState(0, page.Value, pageSize.Value, searchQuery);
                }
                else
                {
                    states = await _mastersService.GetManageState(0, null, null, searchQuery);
                }

                int totalItems = 0;
                if (page.HasValue && pageSize.HasValue)
                {
                    totalItems = await _mastersService.GetTotalStateCount(searchQuery);
                }

                return Json(new { states, totalItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckManageState(string state)
        {
            bool exists = _mastersService.IsManageStateTaken(state);
            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckManageStateCode(int statecode)
        {
            bool exists = _mastersService.IsManageStateCodeTaken(statecode);
            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteManageState(int stateID)
        {
            try
            {
                string message = await _mastersService.DeleteManageState(stateID);
                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Manage City
        [HttpPost]
        public async Task<ActionResult> SaveManageCity(ManageCity manageCity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _mastersService.SaveManageCity(manageCity);
                    return Redirect("/Admin/dist/pages/Masters/ManageCity.html");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(manageCity);
        }

        [HttpGet]
        public async Task<JsonResult> GetManageCity(int? page = null, int? pageSize = null, string searchQuery = null, int? StateCode = null)
        {
            try
            {
                List<ManageCity> cities;
                if (page.HasValue && pageSize.HasValue)
                {
                    cities = await _mastersService.GetManageCity(0, page.Value, pageSize.Value, searchQuery, StateCode);
                }
                else
                {
                    cities = await _mastersService.GetManageCity(0, null, null, searchQuery, StateCode);
                }

                int totalItems = 0;
                if (page.HasValue && pageSize.HasValue)
                {
                    totalItems = await _mastersService.GetTotalCityCount(searchQuery, StateCode);
                }

                return Json(new { cities, totalItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteManageCity(int cityID)
        {
            try
            {
                string message = await _mastersService.DeleteManageCity(cityID);
                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}