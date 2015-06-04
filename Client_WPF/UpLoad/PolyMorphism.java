/*
 * There are two types of polymorphism in java: compile time polymorphism and runtime polymorphism. 
 * We can perform polymorphism in java by method overloading and method overriding.
 * 
 * If you overload static method in java, it is the example of compile time polymorphism.
 * 
 * Runtime polymorphism or Dynamic Method Dispatch is a process in which a 
 * call to an overridden method is resolved at runtime rather than compile-time.
 * 
 * Concept- In this process, an overridden method is called through the reference variable of a superclass. 
 * The determination of the method to be called is based on the object being referred to by the reference variable.
 *
 * We are calling show method of child class using the reference variable of parent class.
 * polyM and poly2 reference variables refers to the object of child class. So it call the show() method of child class
 * */

class ParentClass{
	
	void show()
	{
		System.out.println("Parent class show method");
	}
}

class ChildClass extends ParentClass{
	void show()
	{
		System.out.println("Child class");
	}
	
	void notOverride()
	{
		System.out.println("method defined in child class");
	}
}

public class PolyMorphism {

	
	public static void main(String[] args) {
		ParentClass p = new ParentClass();
		ChildClass c = new ChildClass();
		
		p.show();
		
		ParentClass polyM;
		polyM=c;
		
		polyM.show();
		
		ParentClass poly1 = new ParentClass();
		ParentClass poly2 = new ChildClass();
		
		poly1.show();
		poly2.show();
		
		c.notOverride();
	}

}
