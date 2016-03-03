using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OntologyEditor
{
    [Serializable]
    public class Ontology
    {
        public ClassesList Classes = new ClassesList();
        public PropertiesList Propetries = new PropertiesList();
        public IndividualsList Individuals = new IndividualsList();
        public Diagram diagram = new Diagram();
    }

    [Serializable]
    public class ClassPropertie
    {
        public string objectPropertie;
        public string typeOfPropertie;
        public string destObject;
    }

    [Serializable]
    public class OntoClass
    {
        public string name;
        public string parentName;
        public string annotation;
        public List<ClassPropertie> listOfProperties = new List<ClassPropertie>();
        public List<string> equalClasses = new List<string>();
        public List<string> disjointClasses = new List<string>();
        public List<string> listOfIndividuals = new List<string>();
    }

    [Serializable]
    public class ClassesList : List<OntoClass>
    {
        public OntoClass this[string name]
        {
            get
            {
                return this.First(f => f.name == name);
            }
            set
            {
                this.First(f => f.name == name).annotation = value.annotation;
                this.First(f => f.name == name).parentName = value.parentName;
            }
        }
    }

    [Serializable]
    public class OntoPropertie
    {
        public string name;
        public string parentName;
        public string annotation;
        public bool transitive;
        public bool symmetric;
        public bool reflexive;
        public List<string> diffArea = new List<string>();
        public List<string> valueArea = new List<string>();
        public List<string> equalProperties = new List<string>();
        public List<string> disjointProperties = new List<string>();
    }

    [Serializable]
    public class PropertiesList : List<OntoPropertie>
    {
        public OntoPropertie this[string name]
        {
            get
            {
                return this.First(f => f.name == name);
            }
            set
            {
                this.First(f => f.name == name).annotation = value.annotation;
                this.First(f => f.name == name).parentName = value.parentName;
            }
        }
    }

    [Serializable]
    public class OntoIndividual
    {
        public string name;
        public string annotation;
        public List<string> equalIndividuals = new List<string>();
        public List<string> disjointIndividuals = new List<string>();
        public List<string> listOfClasses = new List<string>();
    }

    [Serializable]
    public class IndividualsList : List<OntoIndividual>
    {
        public OntoIndividual this[string name]
        {
            get
            {
                return this.First(f => f.name == name);
            }
            set
            {
                this.First(f => f.name == name).annotation = value.annotation;
            }
        }
    }
}
