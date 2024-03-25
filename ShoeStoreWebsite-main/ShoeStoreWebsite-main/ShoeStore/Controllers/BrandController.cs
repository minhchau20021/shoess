using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using ShoeStore.DataAccess.Repository.IRepository;
using ShoeStore.Models;
using ShoeStore.Ultitity;
using System.Security.Cryptography;

namespace ShoeStore.Controllers
{
    //l� m?t attribute c?a controller ?? y�u c?u x�c th?c v� ?y quy?n, 
    //ch? cho ph�p ng??i d�ng v?i vai tr� "Admin" truy c?p c�c ph??ng th?c 
    //trong controller.
    [Authorize(Roles = SD.Role_Admin)]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //Constructor n�y nh?n m?t ??i t??ng IUnitOfWork th�ng qua Dependency Injection.IUnitOfWork ???c 
        //s? d?ng ?? t??ng t�c v?i c? s? d? li?u th�ng qua c�c repository.
        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //Ph??ng th?c n�y tr? v? m?t view ch?a danh s�ch t?t c? c�c th??ng hi?u 
        //trong h? th?ng, s? d?ng repository ?? l?y d? li?u.
        // GET: Brand
        public async Task<IActionResult> Index()
        {
              return View(await _unitOfWork.Brands.GetAllAsync());
        }
        //Ph??ng th?c n�y tr? v? view chi ti?t c?a m?t th??ng hi?u d?a tr�n ID.
        //N?u kh�ng t�m th?y th??ng hi?u, tr? v? 404 Not Found.
        // GET: Brand/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
        //Ph??ng th?c n�y tr? v? view ?? t?o m?i m?t th??ng hi?u.
        public IActionResult Create()
        {
            return View();
        }
        //Ph??ng th?c n�y x? l� vi?c t?o m?i th??ng hi?u d?a tr�n d? li?u ???c g?i t? form.
        //S? d?ng[Bind] ?? ch? bind nh?ng tr??ng c?n thi?t.
        //N?u d? li?u h?p l?, th�m th??ng hi?u m?i v�o c? s? d? li?u v� chuy?n h??ng 
        //v? trang danh s�ch th??ng hi?u.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                brand.Created = DateTime.Now;
                brand.Edited = DateTime.Now;
                await _unitOfWork.Brands.AddAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
        //Ph??ng th?c n�y tr? v? view ?? ch?nh s?a th�ng tin c?a m?t th??ng hi?u d?a tr�n ID.
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(e => e.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        //Ph??ng th?c n�y x? l� vi?c c?p nh?t th�ng tin th??ng hi?u d?a tr�n 
        //d? li?u ???c g?i t? form.
        //N?u d? li?u h?p l?, c?p nh?t th??ng hi?u trong c? s? d? li?u v� 
        //chuy?n h??ng v? trang danh s�ch th??ng hi?u.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Brand brand)
        {
            var brandFromDb = await _unitOfWork.Brands.FirstOrDefaultAsync(e => e.Id == id);
            
            if (id != brand.Id || brandFromDb == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                brand.Created = brandFromDb.Created;
                brand.Edited = DateTime.Now;
                _unitOfWork.Brands.Update(brand);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(brand);
        }
        //Ph??ng th?c n�y tr? v? view ?? x�c nh?n x�a m?t th??ng hi?u d?a tr�n ID.
        // GET: Brand/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
        //Ph??ng th?c n�y x? l� vi?c x�a th??ng hi?u d?a tr�n ID.
        //N?u c� c�c m� h�nh gi�y thu?c th??ng hi?u n�y, th�ng b�o l?i v� kh�ng 
        //x�a; ng??c l?i, x�a th??ng hi?u v� chuy?n h??ng v? trang danh s�ch 
        //th??ng hi?u.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(e => e.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            var shoes = await _unitOfWork.Shoes.GetAllAsync(e => e.BrandId == brand.Id);
            if(shoes.Count == 0)
            {
                _unitOfWork.Brands.Remove(brand);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                TempData[SD.Error] = "Some shoe models is belong to this Brand. Can not delete it!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
