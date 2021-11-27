using System.Xml;

namespace ConsoleVideo {
    internal class VariableReferenceBlock {
        internal XmlNode GeneratedVariableReferenceBlock {
            get;
        }

        internal VariableReferenceBlock(XmlDocument xmlDocument,
                                        Variable variable,
                                        XmlNode objectInstance = null) {
            //VariableReferenceBlock
            GeneratedVariableReferenceBlock = xmlDocument.CreateElement("block");

            {
                //VariableReferenceBlock.type.
                XmlAttribute variableReferenceBlockType = xmlDocument.CreateAttribute("type");
                variableReferenceBlockType.Value = "variableReferenceBlock";
                GeneratedVariableReferenceBlock.Attributes?.Append(variableReferenceBlockType);
            }

            bool isObjectVar = (variable.VariableType != VariableType.Global);
            {
                //VariableReferenceBlock.mutation
                XmlNode variableReferenceBlockMutation = xmlDocument.CreateElement("mutation");
                GeneratedVariableReferenceBlock.AppendChild(variableReferenceBlockMutation);

                {
                    //VariableReferenceBlock.mutation.isObjectVar
                    XmlAttribute variableReferenceBlockBlockMutationIsObjectVar = xmlDocument.CreateAttribute("isObjectVar");
                    variableReferenceBlockBlockMutationIsObjectVar.Value = isObjectVar.ToString()
                                                                                      .ToLower();
                    variableReferenceBlockMutation.Attributes?.Append(variableReferenceBlockBlockMutationIsObjectVar);
                }
            }

            {
                //VariableReferenceBlock.objectType.
                XmlNode variableReferenceBlockObjectType = xmlDocument.CreateElement("field");
                variableReferenceBlockObjectType.InnerText = variable.VariableType.ToString();
                GeneratedVariableReferenceBlock.AppendChild(variableReferenceBlockObjectType);

                {
                    //VariableReferenceBlock.objectType.name.
                    XmlAttribute variableReferenceBlockObjectTypeName = xmlDocument.CreateAttribute("name");
                    variableReferenceBlockObjectTypeName.Value = "OBJECTTYPE";
                    variableReferenceBlockObjectType.Attributes?.Append(variableReferenceBlockObjectTypeName);
                }
            }

            {
                //VariableReferenceBlock.var.
                XmlNode variableReferenceBlockVar = xmlDocument.CreateElement("field");
                variableReferenceBlockVar.InnerText = variable.Name;
                GeneratedVariableReferenceBlock.AppendChild(variableReferenceBlockVar);

                {
                    //VariableReferenceBlock.var.name.
                    XmlAttribute variableReferenceBlockVarName = xmlDocument.CreateAttribute("name");
                    variableReferenceBlockVarName.Value = "VAR";
                    variableReferenceBlockVar.Attributes?.Append(variableReferenceBlockVarName);

                    //VariableReferenceBlock.var.id.
                    XmlAttribute variableReferenceBlockVarId = xmlDocument.CreateAttribute("id");
                    variableReferenceBlockVarId.Value = variable.Id;
                    variableReferenceBlockVar.Attributes?.Append(variableReferenceBlockVarId);

                    //VariableReferenceBlock.var.variableType.
                    XmlAttribute variableReferenceBlockVarVariableType = xmlDocument.CreateAttribute("variabletype");
                    variableReferenceBlockVarVariableType.Value = variable.VariableType.ToString();
                    variableReferenceBlockVar.Attributes?.Append(variableReferenceBlockVarVariableType);
                }
            }

            {
                if ((!isObjectVar) || (objectInstance == null)) {
                    return;
                }

                //VariableReferenceBlock.object.
                XmlNode variableReferenceBlockObject = xmlDocument.CreateElement("value");
                variableReferenceBlockObject.AppendChild(objectInstance);
                GeneratedVariableReferenceBlock.AppendChild(variableReferenceBlockObject);

                {
                    //VariableReferenceBlock.object.name.
                    XmlAttribute variableReferenceBlockObjectName = xmlDocument.CreateAttribute("name");
                    variableReferenceBlockObjectName.Value = "OBJECT";
                    variableReferenceBlockObject.Attributes?.Append(variableReferenceBlockObjectName);
                }
            }

            return;
        }
    }
}