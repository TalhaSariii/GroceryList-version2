//using GroceryList.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace GroceryList.Services
//{
//    public class ShoppingListAppService : IShoppingListAppService
//    {
//        private readonly Context _context;
//        public ShoppingListAppService(Context context) { 
//            _context = context;
//        }
//        public bool isItemExists(int Id)
//        {
//            var deneme = _context.Items.Where(x => x.Id == Id);
//            if(deneme != null)
//            {
//                return true;
//            }
//            else { return false; }
//        }
//    }
//}
