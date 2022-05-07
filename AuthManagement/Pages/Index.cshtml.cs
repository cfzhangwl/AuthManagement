using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStringLocalizer<Menu> _localizer;

        public IndexModel(IStringLocalizer<Menu> localizer,ILogger<IndexModel> logger)
        {
            _logger = logger;
            _localizer = localizer;
        }
        public string LocalizerTest;

        public void OnGet()
        {
            LocalizerTest = _localizer["SysTitle"]; //直接使用 key 值引用就可以了。
        }
    }
}
