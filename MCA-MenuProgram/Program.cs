using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MCA_MenuProgram
{
    public class MenuLinks
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public int? ParentId { get; set; }
        public bool isHidden { get; set; }
        public string LinkURL { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PrintMenuList();
        }

        private static void PrintMenuList()
        {
            var menuList = new StreamReader(@"C:\Users\ljafc\source\repos\MCA-MenuProgram\MCA-MenuProgram\Navigation.csv");
            var readMenu = menuList.ReadToEnd();
            var splitMenu = readMenu.Split(';');

            var menuLinksList = GetMenuList(splitMenu);
            var orderedMenuList = OrderMenuListByParentId(menuLinksList);

            foreach (var item in orderedMenuList)
            {
                var linkUrl = item.LinkURL.Split('/');
                if (linkUrl.Length == 2 && item.isHidden == false)
                {
                    Console.WriteLine(" - " + item.MenuName);
                }
                else if (linkUrl.Length == 3 && item.isHidden == false)
                {
                    Console.WriteLine(" - - - -  " + item.MenuName);
                }
                else if (linkUrl.Length == 4 && item.isHidden == false)
                {
                    Console.WriteLine(" - - - - - - -  " + item.MenuName);
                }
            }
        }
        public static List<MenuLinks> GetMenuList(string[] list)
        {
            var menuLinksList = new List<MenuLinks>();
            for (int i = 5; i <= list.Length - 2; i = i + 5)
            {
                var menu = new MenuLinks();
                menu.Id = Convert.ToInt32(list[i]);
                menu.MenuName = list[i + 1];

                if (list[i + 2] == "NULL")
                    menu.ParentId = null;
                else
                    menu.ParentId = Convert.ToInt32(list[i + 2]);

                if (list[i + 3] == "False")
                    menu.isHidden = false;
                else
                    menu.isHidden = true;

                menu.LinkURL = list[i + 4];
                menuLinksList.Add(menu);
            }
            return menuLinksList;
        }
        private static List<MenuLinks> OrderMenuListByParentId(List<MenuLinks> menuLinksList)
        {
            menuLinksList = menuLinksList.OrderBy(x => x.ParentId).ThenBy(x => x.MenuName).ToList();

            var stack = new Stack<MenuLinks>();

            foreach (var link in menuLinksList.Where(x => x.ParentId == null).Reverse())
            {
                stack.Push(link);
                menuLinksList.RemoveAt(0);
            }

            var output = new List<MenuLinks>();

            while (stack.Any())
            {
                var currentSection = stack.Pop();

                var children = menuLinksList.Where(x => x.ParentId == currentSection.Id).Reverse();

                foreach (var section in children)
                {
                    stack.Push(section);
                    menuLinksList.Remove(section);
                }
                output.Add(currentSection);
            }
            return output;
        }
    }
}
