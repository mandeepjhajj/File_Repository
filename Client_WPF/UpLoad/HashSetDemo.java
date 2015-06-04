import java.util.*;

class Student
{
	int age;
	String name;
	Student(int age, String name)
	{
		this.age=age;
		this.name=name;
	}
	/*
	 * hashCode function is over ridden 
	 * It is a defined by AbstractSet class and if we do not over ride, the hashCode will be calculated internally
	 * */
	public int hashCode()
	{
		return age;
	}
}
public class HashSetDemo
{
   public static void main(String[] args)
   {
      //String[] colors = {"white", "pink", "red", "green", "red", "orange","Aa","BB"};
	   
	  Student s1 = new Student(14,"Mandeep");
	  Student s2 = new Student(28,"Jasdeep");
	  Student sdup = new Student(28,"Jasdeep");
	  Student s3 = new Student(30,"Didar");
	  
      HashSet<Student> hs = new HashSet<Student>();

     // for(int i = 0; i < colors.length; i++)  hs.add(colors[i]);

      hs.add(s1);
      hs.add(s2);
      hs.add(s3);
      hs.add(sdup); // its added since its not duplicate element. Its a different object.
      hs.add(s3); //duplicate not added to hashSet
      
      System.out.println(hs);

     // System.out.println("Does it contain green?  " + hs.contains("green"));

      Iterator<Student> itr = hs.iterator();

      while(itr.hasNext())
      {
         System.out.print(itr.next().name+" ");
      }
      System.out.println();


      for(Student str : hs)
         System.out.print(str.age +" ");
      System.out.println();
   }
}