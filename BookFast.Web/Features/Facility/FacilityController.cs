using System;
using System.Threading.Tasks;
using BookFast.Web.Contracts;
using BookFast.Web.Contracts.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookFast.Web.Features.Facility.ViewModels;

namespace BookFast.Web.Features.Facility
{
    [Authorize(Policy = "FacilityProviderOnly")]
    public class FacilityController : Controller
    {
        private readonly IFacilityService facilityService;
        private readonly FacilityMapper facilityMapper;

        private readonly IAccommodationService accommodationService;
        private readonly AccommodationMapper accommodationMapper;

        public FacilityController(IFacilityService facilityService, FacilityMapper facilityMapper, 
            IAccommodationService accommodationService, AccommodationMapper accommodationMapper)
        {
            this.facilityService = facilityService;
            this.facilityMapper = facilityMapper;
            this.accommodationService = accommodationService;
            this.accommodationMapper = accommodationMapper;
        }

        public async Task<IActionResult> Index()
        {
            var facilities = await facilityService.ListAsync();
            return View(facilityMapper.MapFrom(facilities));
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            try
            {
                var facility = await facilityService.FindAsync(id.Value);
                var accommodations = await accommodationService.ListAsync(facility.Id);
                return View(new FacilityDetailsViewModel
                            {
                                Facility = facilityMapper.MapFrom(facility),
                                Accommodations = accommodationMapper.MapFrom(accommodations)
                            });
            }
            catch (FacilityNotFoundException)
            {
                return View("NotFound");
            }
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacilityViewModel facilityViewModel)
        {
            if (ModelState.IsValid)
            {
                var details = facilityMapper.MapFrom(facilityViewModel);

                await facilityService.CreateAsync(details);
                return RedirectToAction("Index");
            }
            return View(facilityViewModel);
        }
        
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            try
            {
                var facility = await facilityService.FindAsync(id.Value);
                return View(facilityMapper.MapFrom(facility));
            }
            catch (FacilityNotFoundException)
            {
                return View("NotFound");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FacilityViewModel facilityViewModel)
        {
            if (ModelState.IsValid)
            {
                var details = facilityMapper.MapFrom(facilityViewModel);

                try
                {
                    await facilityService.UpdateAsync(facilityViewModel.Id, details);
                    return RedirectToAction("Index");
                }
                catch (FacilityNotFoundException)
                {
                    return View("NotFound");
                }
            }

            return View(facilityViewModel);
        }
        
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            try
            {
                var facility = await facilityService.FindAsync(id.Value);
                return View(facilityMapper.MapFrom(facility));
            }
            catch (FacilityNotFoundException)
            {
                return View("NotFound");
            }
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await facilityService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (FacilityNotFoundException)
            {
                return View("NotFound");
            }
        }
    }
}
