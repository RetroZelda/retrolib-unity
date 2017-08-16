using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Retro.Events
{
    public class Chapter 
    {
        private class PageHandleContainer
        {
            public Action<IPage> PageAction;
        }
        // variables
        private Dictionary<Type, List<IReader>> _PageSubscriptions;
        private Dictionary<Type, PageHandleContainer> _PageActions;
	    private Queue<IPage> _PageQueue;
	    private int _ID;

	    public int getID() { return _ID; }
	    public void setID(int _ID) { this._ID = _ID; }

	    // Private methods
	    private void ChapterCreate(int nID)
        {
            _PageSubscriptions = new Dictionary<Type, List<IReader>>();
            _PageActions = new Dictionary<Type, PageHandleContainer>();
		    _PageQueue = new Queue<IPage>();
		    _ID = nID;
		    Debug.Log("Chapter Created: " + nID);
	    }
	
	    // public methods
	    public Chapter(String szID)
	    {
		    ChapterCreate(szID.GetHashCode());
	    }
	    public Chapter(int nID)
	    {
		    ChapterCreate(nID);
	    }

	    // listed for a page
	    public void Subscribe<T>(IReader reader) where T : IPage
	    {
            Type pageType = typeof(T);
		    // create the new list if not already in existence
		    if(_PageSubscriptions.ContainsKey(pageType) == false)
		    {
			    _PageSubscriptions.Add(pageType, new List<IReader>());
		    }
		
		    // add
		    _PageSubscriptions[pageType].Add(reader);
		    Debug.Log(reader.ToString() + " Subscribed to: " + _PageSubscriptions[pageType].ToString());
	    }
	
	    // stop listening for a page
        public void Unsubscribe<T>(IReader reader) where T : IPage
        {
            Type pageType = typeof(T);
		    // remove
		    _PageSubscriptions[pageType].Remove(reader);
		    Debug.Log(reader.ToString() + " Unsubscribed from: " + _PageSubscriptions[pageType].ToString());
	    }

        private Action<IPage> ActionConvert<T>(Action<T> action)
        {
            return new Action<IPage>(o => action((T)o));
        }

        // listed for a page
        public void Subscribe<T>(Action<IPage> action) where T : IPage
        {
            Type pageType = typeof(T);

            // create the new list if not already in existence
            if (_PageActions.ContainsKey(pageType) == false)
            {
                _PageActions.Add(pageType, new PageHandleContainer());
            }
            _PageActions[pageType].PageAction += action;

            // add
            Debug.Log("Subscribed Action to: " + action.ToString());
        }

        // stop listening for a page
        public void Unsubscribe<T>(Action<IPage> action) where T : IPage
        {
            Type pageType = typeof(T);

            // remove
            _PageActions[pageType].PageAction -= action;
            Debug.Log("Unsubscribed Action from: " + action.ToString());
        }
	
	    // distribute new page to all readers now
	    public void Set(IPage page)
	    {
		    // send to all readers
		    if(_PageSubscriptions.ContainsKey(page.GetType()))
		    {
			    List<IReader> readers = _PageSubscriptions[page.GetType()];
			    for(int nReaderIndex = 0; nReaderIndex < readers.Count; ++nReaderIndex)
			    {
				    readers[nReaderIndex].ReadPage(page);
			    }
		    }

            // call all actions
            if (_PageActions.ContainsKey(page.GetType()))
            {
                _PageActions[page.GetType()].PageAction(page);
            }
	    }
	
	    // mark new page to be distributed soon
	    public void Mark(IPage page)
	    {
		    _PageQueue.Enqueue(page);
	    }
	
	    // process each queued page
	    public void Process()
	    {
		    while(_PageQueue.Count > 0)
		    {
			    Set(_PageQueue.Dequeue());
		    }
	    }
    }
}
