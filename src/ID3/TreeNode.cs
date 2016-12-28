using System.Collections.Generic;

namespace ID3
{
    public class TreeNode
    {
        public bool? Result { get; set; }

        public int Attribute { get; set; }

        /// <summary>
        /// The keys of the dictionary reprenset the possible values for the attribute, 
        /// the Values of the dictionary represent the possible states if each value is chosen
        /// </summary>
        public IDictionary<string, TreeNode> SubNodes { get; set; }

    }
}
