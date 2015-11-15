using System;

namespace Demo.Models
{
    public class Sample
    {
        public SideBarImage Category { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }
    }

    public enum SideBarImage
    {
        Input,
        Commands,
        Container,
    }
}
