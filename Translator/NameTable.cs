using LexicalAnalise;
using SyntaxAnalyzer;
using System.CodeDom.Compiler;
using Translation;
using Translator;

namespace NameTables;

public  class NameTable
{
	private static List<Identifier> _identifiers;

    public  static List<Identifier> Identifiers => _identifiers;

	 static NameTable()
	{
		_identifiers = new List<Identifier>();
	}


    public static void AddIdentifier(string name, tCat category)
	{
		if (FindIdentifierByName(name) != null)
			throw new Exception("Ошибка! Идентификатор с таким именем уже существует!");

		_identifiers.Add(new Identifier(name, category));
	}

	public static void AddIdentifier(string name, tCat category, tType type)
	{
		if (FindIdentifierByName(name) != null)
			throw new Exception("Ошибка! Идентификатор с таким именем уже существует!");

		_identifiers.Add(new Identifier(name, category, type));
	}

	public static  Identifier? FindIdentifierByName(string name)
	{
		foreach (Identifier identifier in _identifiers) 
			if (identifier.name == name)
				return identifier;

		return null;
	}

}
