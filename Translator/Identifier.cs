using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    public struct Identifier
    {
        public string name;
        public tCat category;
        public tType type;

        public Identifier(string name, tCat category, tType type)
        {
            this.name = name;
            this.category = category;
            this.type = type;
        }

        public Identifier(string name, tCat category) : this(name, category, tType.None)
        {
        }

        public Identifier() : this(string.Empty, tCat.Const, tType.None)
        {
        }
    }
    public enum tCat
    {
        Const,
        Var,
        Type
    }
    public enum tType
    {
        None,
        Int,
        Bool
    }


}

