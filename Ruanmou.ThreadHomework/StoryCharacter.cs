using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruanmou.ThreadHomework
{
    /// <summary>
    /// 故事角色类
    /// </summary>
    public class StoryCharacter
    {
        public string Name { get; set; }
        public ConsoleColor Color { get; set; }
        /// <summary>
        /// 经历
        /// </summary>
        public List<string> Experience { get; set; }
    }
}
