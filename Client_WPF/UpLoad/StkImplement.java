/*
 * Stack is implementation of List interface.
 * In this case we are manipulating the string elements
 * Parent class is not aware of the methods which are defined by child class.
 * So we need to downcast the reference variable to Stack class to use PUSH and POP.
 */

import java.util.*;

public class StkImplement {

	public static void main(String[] args) {
		List<String> stk = new Stack<String>();
		
		StkImplement s = new StkImplement();
		
		System.out.println("Adding elements to stack");
		s.addElem("mandeep",stk);
		s.addElem("singh",stk);
		s.addElem("jhajj",stk);
		
		s.removeElem(stk);
		s.removeElem(stk);
		s.removeElem(stk);
		s.removeElem(stk);
		
	}
	public void addElem(String a,List<String> l)
	{
		Stack<String> s1 =(Stack<String>)l;
		s1.push(a);
		System.out.println("Element added to the stack -> "+a);
	}
	public void removeElem(List<String> l)
	{
		Stack<String> s1 =(Stack<String>)l;
		if(s1.isEmpty())
		{
			System.out.println("Stack is empty");
		}
		else
		{
			String s = s1.pop();
		    System.out.println("Popped element from stack -> "+s);
		}
	}
}
