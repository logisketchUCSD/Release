using System;
using System.Collections.Generic;

namespace CommandManagement
{
	/// <summary>
	/// Summary description for CommandManager.
	/// </summary>
	public class CommandManager
	{
		/// <summary>
		/// Stack of Commands that can be Undone
		/// </summary>
		private Stack<Command> undoCommands;

		/// <summary>
		/// Stack of Commands that can be Redone
		/// </summary>
		private Stack<Command> redoCommands;

		
		/// <summary>
		/// Constructor
		/// 
		/// Utilizes Undo and Redo stacks to handle Commands
		/// </summary>
		public CommandManager()
		{
            undoCommands = new Stack<Command>();
			redoCommands = new Stack<Command>();
		}
        

		/// <summary>
		/// Executes a Command and places it onto the Undo stack, if appropriate.
		/// </summary>
		/// <param name="command">Command to execute</param>
		public void ExecuteCommand(Command command)
		{
			command.Execute();
			
			if (command.IsUndoable())
			{
				undoCommands.Push(command);
				redoCommands.Clear();
			}
		}


		/// <summary>
		/// Undos the last (undoable) Command.
		/// </summary>
		/// <returns>True if a Command is unexecuted</returns>
		public bool Undo()
		{
			if (undoCommands.Count > 0)
			{
				Command currCommand = undoCommands.Pop();
				currCommand.UnExecute();

				redoCommands.Push(currCommand);

				return true;
			}
			else
			{
				return false;
			}
		}
		

		/// <summary>
		/// Redos the last undone Command.
		/// </summary>
		/// <returns>True if a Command is re-executed</returns>
		public bool Redo()
		{
			if (redoCommands.Count > 0)
			{
				Command currCommand = redoCommands.Pop();
				currCommand.Execute();

				undoCommands.Push(currCommand);

				return true;
			}
			else
			{
				return false;
			}
		}
	

		/// <summary>
		/// Clears both the Undo and Redo stacks.
		/// </summary>
		public void ClearStacks()
		{
			undoCommands.Clear();
			redoCommands.Clear();
		}
	}
}
