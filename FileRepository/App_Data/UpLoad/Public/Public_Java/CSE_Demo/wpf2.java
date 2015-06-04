//////////////////////////////////////////////////////////////////////////////
// TrinaryTree.java  -Build and operate on Trinary tree                		//
//                    Basic insert and delete operation in Trinary tree		//
// 																	   		//
// Version 1.0                                                         		//
// Language:  Java									               	     	//
// Platform:  HP Pavilion dv6 , Win 7, SP 1                          		//
// Author:    Mandeep Singh, Syracuse University                            //
//            315-751-3413, mjhajj@syr.edu or mandeepsinghjhajj1@gmail.com  //
//////////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module define the following classes
 *   TriNode  - Defines the node in tree.
 *   TrinaryTree  - Provide the function to manipulate the tree.
 * 
 * Public Interface
 * ================
 * 
 * public void insert(int value);
 * public void delete(TriNode root, int key);
 * public void print(TriNode root);
 *
 * Maintenance History
 * ===================
 * 
 * Version 1.0 : 01 April 2015
 * - First release
 * 
 */


/* Interface that provide the operations allowed on tree manipulation*/
 interface ITrinaryTree{
	 public void insert(int value);
	 public void delete(TriNode root, int key);
	 public void print(TriNode root);
	 
 }
 
 /* Node includes the left, right and equal pointers. Each node has add and remove functions. 
  * Nodes are not having parent pointer. Instead I am searching for the parent at run time. 
  * This might increase the overall complexity of program. But I want to make my program 
  * independent of whether parent pointer is there or not.
  * 
  * Also I could have included the main functionality of insert and delete in TrinaryTree tree
  * that would have been much easier way to code and to understand that recursion.
  * But I included my computational logic in TriNode class because I want to hide the logic
  * of manipulation from other classes. So that they only need to call add or remove function,
  * and rest all is taken care by my TriNode class*/
 
 class TriNode{
	private int data;
	private TriNode left;
	private TriNode right;
	private TriNode equal;
	private static int rootVisit=0;
	static TriNode head = null;
	
	TriNode(int data){
		this.data=data;
		left=right=equal=null;
	}
	
	public int getKey(){
		return this.data;
	}
	
	public TriNode getLeft(){
		return this.left;
	}
	
	public TriNode getRight(){
		return this.right;
	}
	public TriNode getEqual(){
		return this.equal;
	}
	public int getVisit(){
		return rootVisit;
	}
	
	public void setKey(int value){
		this.data=value;
	}
	
	public void setLeft(TriNode node){
		this.left=node;
	}
	
	public void setRight(TriNode node){
		this.right=node;
	}
	
	public void setEqual(TriNode node){
		this.equal=node;
	}
	
	public void setVisit(){
		rootVisit=1;
	}
	
	/* Traverse and find the parent of node */
	public TriNode getParent(TriNode head, TriNode previous, int value){
				
		if(head==null){
			return null;
		}
		
		else{
			previous = head;
			while(head.getKey()!=value && head!=null){
				
				previous = head;
				if(head.getKey()< value){
					head = head.getRight();
				}
				else if( head.getKey() > value){
					head = head.getLeft();
				}
				else{
					head = head.getEqual();
				}
			}
		}
		return previous;
	}
	
	/* Main logic to inserting node in tree*/
	public void add(int newValue){
		
		if(this.getKey()>newValue){
			if(this.getLeft()!=null){
				this.getLeft().add(newValue);
			}
			else{
				this.setLeft(new TriNode(newValue));
			}
		}
		else if( this.getKey()<newValue){
			if(this.getRight()!=null){
				this.getRight().add(newValue);
			}
			else{
				this.setRight(new TriNode(newValue));
			}
		}
		else{
			if(this.getEqual()!=null){
				this.getEqual().add(newValue);
			}
			else{
				this.setEqual(new TriNode(newValue));
			}
		}
	}
	
	/* Logic for removing node */
	public void remove(int value){
				
		if(getVisit()==0){
			head= this;
			setVisit();
		}
		if(this.getKey() == value){
			TriNode previous = null;
			//leaf node
			if(this.getLeft()==null && this.getEqual()==null && this.getRight()==null){
				
				TriNode parent = this.getParent(head, previous, value);
				modifyLinksForOneOrNoChild(this,parent);
			}
			//Recurse on node with equal pointer
			else if(this.getEqual()!= null){
				this.getEqual().remove(value);
			}
			else{
				
				// Single child either left or right
				if(this.getRight()== null || this.getLeft()==null){
					TriNode parent = this.getParent(head, previous, value);
					modifyLinksForOneOrNoChild(this,parent);
				}
				else{
					//When left and right child are present
					int nextValue =findSuccessor(this.getRight()).getKey();
					findSuccessor(this.getRight()).remove(findSuccessor(this.getRight()).getKey());
					this.setKey(nextValue);
				}
			}
		}
		else if( this.getKey() > value){
			if(this.getLeft()!=null){
			this.getLeft().remove(value);
			}
		}
		else{
			if(this.getRight()!= null){
				this.getRight().remove(value);
			}
		}
	}
	
	/*Set the links appropriate, it is used to move the left or right child at appropriate position of its parent.*/
	public void modifyLinksForOneOrNoChild(TriNode toDelete, TriNode parent){
		
		TriNode setProper = toDelete.getLeft()==null? toDelete.getRight() : toDelete.getLeft();
		if(toDelete == parent.getEqual()){
			parent.setEqual(setProper);
		}
		else if(toDelete == parent.getLeft()){
			parent.setLeft(setProper);
		}
		else{
			parent.setRight(setProper);
		}
	}
	
	/* Find the successor of the given node*/
	public TriNode findSuccessor(TriNode node){
		while(node.getLeft()!= null){
			node= node.getLeft();
		}
		return node;
	}
}
 
 
public class TrinaryTree implements ITrinaryTree{

	private static TriNode root=null;
	
	/* Inserts the node at appropriate place by calling insert on 
	 * particular node. Each node is responsible to identify the path for 
	 * inserting the new node.*/
	public void insert(int newValue){
		if(root==null){
			root= new TriNode(newValue);
		}
		else{
			root.add(newValue);
		}
	}
	
	/* This function call the remove on the root node. Each node is responsible to identify 
	 * the appropriate node on which remove should be called as per tree structure*/
	
	public void delete(TriNode root, int key){
		if (root == null)
			return;
		else{
			root.remove(key);
		}
	}
	
	/* Prints the tree in increasing order */
	public void print(TriNode root) {
	    if (root != null) {
	    	print(root.getLeft());
	    	
	      print(root.getEqual());
	      System.out.println("Node : " + root.getKey());
	      print(root.getRight());
	    }
	  }
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		ITrinaryTree tree = new TrinaryTree();
        tree.insert(5);
        tree.insert(4);
        tree.insert(9);
        tree.insert(5);
        tree.insert(7);
        tree.insert(2);
        tree.insert(2);

        System.out.println("Print tree in increasing order");
        tree.print(root);

        tree.delete(root,5);
        tree.delete(root,9);
        tree.delete(root, 4);
        
        System.out.println("\nTree left after deleting nodes");
        
        tree.print(root);
	}

}
