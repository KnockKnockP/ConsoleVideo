namespace ConsoleVideo {
    internal class Variable {
        internal string Name {
            get;
        }

        internal string Id {
            get;
        }

        internal VariableType VariableType {
            get;
        }

        internal Variable(string name,
                          string id,
                          VariableType variableType) =>
            (Name, Id, VariableType) = (name, id, variableType);
    }
}