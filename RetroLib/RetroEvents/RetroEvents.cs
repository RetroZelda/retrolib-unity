using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Retro.Events
{
    public class RetroEvents
    {
        // our data
        private Dictionary<int, Chapter> _Chapters;

        // singleton
        private static RetroEvents Instance = new RetroEvents();
        private RetroEvents()
        {
            _Chapters = new Dictionary<int, Chapter>();
        }
        /////////////////////////////////////////////////////////

        // private functions
        private Chapter GetChapter(int nID)
        {
            if (_Chapters.ContainsKey(nID) == false)
            {
                _Chapters.Add(nID, new Chapter(nID));
            }
            return _Chapters[nID];
        }

        public void Process_Internal()
	    {
		    foreach(Chapter chapter in _Chapters.Values)
		    {
			    chapter.Process();
		    }
	    }

        // public static bridge functions
        public static Chapter Chapter(int nID)
        {
            return Instance.GetChapter(nID);
        }

        public static Chapter Chapter(String szID)
        {
            return Instance.GetChapter(szID.GetHashCode());
        }

        public static void Process()
        {
            Instance.Process_Internal();
        }
    }
}