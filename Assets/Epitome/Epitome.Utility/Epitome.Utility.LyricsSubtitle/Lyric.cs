using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LyricItem : IComparable<LyricItem>
{
    public const Int64 INVALID_TIME_STAMP = Int64.MaxValue;

    // lyric text
    public string mText = "";

    // lyric timestamp in millisecond
    public Int64 mTimestamp = 0;

    // next lyric timestamp in millisecond
    public Int64 mNextTimestamp = INVALID_TIME_STAMP;

    // the lyric index when all lyric sorted by timestamp in ascending order
    public int mIndex = 0;

    public LyricItem() { }
    public LyricItem(string text, Int64 timestamp)
    {
        mText = text;
        mTimestamp = timestamp;
    }
    public int CompareTo(LyricItem other)
    {
        if (mTimestamp == other.mTimestamp)
            return 0;

        if (mTimestamp < other.mTimestamp)
            return -1;

        return 1;
    }
}

public class Lyric  {
    // those ID Tags are opitional
    
    // [ti : value]
    // Lyrics (song) title
    public string mTitle = "";

    // [ar : value]
    // name of artist
    public string mArtist = "";

    // [al : value]
    // Album where the song is from
    public string mAlbum = "";

    // [au : value]
    // Creator of the Songtext
    public string mAuthor = "";

    // [by : value]
    // name of lyric file creator
    public string mBy = "";

    // [offset : value]
    // offset of all lyric timestamp
    // integer in millisecond
    public Int64 mOffset = 0;

    // [length : value]
    // How long the song is
    public string mLength = "";

    // lyric data
    protected List<LyricItem> mItems = new List<LyricItem>();

    // ID tag key
    protected const string STRING_ID_TAG_TITLE = "ti";
    protected const string STRING_ID_TAG_ARTIST = "ar";
    protected const string STRING_ID_TAG_ALBUM = "al";
    protected const string STRING_ID_TAG_AUTHOR = "au";
    protected const string STRING_ID_TAG_BY = "by";
    protected const string STRING_ID_TAG_OFFSET = "offset";
    protected const string STRING_ID_TAG_LENGTH = "length";

    // now the lrc file is need UTF-8 encode
    public bool Load(string path)
    {
        mItems.Clear();
        if (null == path)
        {
            LyricLogError("file path is null");
            return false;
        }
        if (path.Length <= 0 || !System.IO.File.Exists(path))
        {
            LyricLogError(path + " not exist");
            return false;
        }
        LyricLog("Load lyric begin : " + path);

        System.IO.StreamReader streamReader = new System.IO.StreamReader(path);
        string line = null;
        while (true)
        {
            line = streamReader.ReadLine();
            if (null == line)
                break;

            ParseLine(line);
        }

        UpdateTimestamp();

        LyricLog("Load lyric end");

        return true;
    }

    // search current lyric, timestamp in [item.mTimestamp, item.mNextTimestamp)
    public LyricItem SearchCurrentItem(Int64 timestamp)
    {
        LyricItem item = null;
        foreach (LyricItem it in mItems)
        {
            if (timestamp >= it.mTimestamp && timestamp < it.mNextTimestamp)
            {
                item = it;
                break;
            }
        }
        return item;
    }

    // get all lyric data
    public List<LyricItem> GetItems()
    {
        return mItems;
    }

    // add the RGB info for string
    public static string WrapStringWithColorTag(string str, int R, int G, int B)
    {
        if (null == str)
            return "";

        R = Math.Max(Math.Min(R, 255), 0);
        G = Math.Max(Math.Min(G, 255), 0);
        B = Math.Max(Math.Min(B, 255), 0);

        object[] param = { R, G, B, str };
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", param);
    }

    protected bool ParseLine(string line)
    {
        if (null == line)
            return false;

        LyricLogDebug("current line: " + line);

        int notFoundIndex = -1;
        char openBracket = '[', closeBracket = ']';

        int openBracketIndex = 0, closedBracketIndex = 0;
        int startSearchIndex = 0;
        List<Int64> timestampList = new List<Int64>();
        while (true)
        {
            if (startSearchIndex >= line.Length)
                break;

            openBracketIndex = line.IndexOf(openBracket, startSearchIndex);
            if (notFoundIndex == openBracketIndex)
                break;

            closedBracketIndex = line.IndexOf(closeBracket, openBracketIndex);
            if (notFoundIndex == closedBracketIndex || closedBracketIndex - openBracketIndex < 2)
                break;

            string tagString = line.Substring(openBracketIndex + 1, closedBracketIndex - openBracketIndex - 1);
            LyricLogDebug("tagString: " + tagString);

            string[] tagSplitArray = tagString.Split(':');
            foreach (string str in tagSplitArray)
            {
                LyricLogDebug("tagSplitArray: " + str);
            }

            if (!TryParseIDTag(tagString, tagSplitArray))
            {
                Int64 timestamp = 0;
                if (TryParseTimeTag(tagString, tagSplitArray, out timestamp))
                {
                    timestampList.Add(timestamp);
                }
            }

            startSearchIndex = closedBracketIndex + 1;
        }

        if (timestampList.Count > 0)
        {
            string text = "";
            if (startSearchIndex < line.Length)
            {
                text = line.Substring(startSearchIndex);
            }
            LyricLogDebug("lyric text: " + text);
            foreach (Int64 timestamp in timestampList)
            {
                mItems.Add(new LyricItem(text, timestamp));
            }
        }

        return true;
    }

    protected bool TryParseIDTag(string tagString, string[] tagSplitArray)
    {
        if (null == tagSplitArray || tagSplitArray.Length < 2)
            return false;

        // is need trim ?
        string tagName = tagSplitArray[0].Trim();
        if (tagName.Equals(STRING_ID_TAG_TITLE, StringComparison.CurrentCultureIgnoreCase))
        {
            mTitle = tagSplitArray[1];
        }
        else if (tagName.Equals(STRING_ID_TAG_ARTIST, StringComparison.CurrentCultureIgnoreCase))
        {
            mArtist = tagSplitArray[1];
        }
        else if (tagName.Equals(STRING_ID_TAG_ALBUM, StringComparison.CurrentCultureIgnoreCase))
        {
            mAlbum = tagSplitArray[1];
        }
        else if (tagName.Equals(STRING_ID_TAG_AUTHOR, StringComparison.CurrentCultureIgnoreCase))
        {
            mAuthor = tagSplitArray[1];
        }
        else if (tagName.Equals(STRING_ID_TAG_BY, StringComparison.CurrentCultureIgnoreCase))
        {
            mBy = tagSplitArray[1];
        }
        else if (tagName.Equals(STRING_ID_TAG_OFFSET, StringComparison.CurrentCultureIgnoreCase))
        {
            Int64 offset = 0;
            if (!Int64.TryParse(tagSplitArray[1], out offset))
                return false;

            mOffset = offset;
        }
        else if (tagName.Equals(STRING_ID_TAG_LENGTH, StringComparison.CurrentCultureIgnoreCase))
        {
            mLength = tagSplitArray[1];
        }
        else
        {
            // not valid now
            return false;
        }

        return true;
    }

    protected bool TryParseTimeTag(string tagString, string[] tagSplitArray, out Int64 timestamp)
    {
        timestamp = 0;
        if (null == tagSplitArray || tagSplitArray.Length < 2)
            return false;

        if (null == tagString)
            return false;

        char[] separator = { ':', '.' };
        string[] timeSplitArray = tagString.Split(separator);
        if (null == timeSplitArray || (timeSplitArray.Length != 2 && timeSplitArray.Length != 3))
            return false;

        // timeSplitArray.Length == 2, [minute : second]
        // timeSplitArray.Length == 3, [minute : second : xx] or [minute : second . xx]
        // xx is hundredths of a second

        Int64 minute = 0, second = 0, millisecond = 0;
        if (!Int64.TryParse(timeSplitArray[0], out minute))
            return false;

        if (!Int64.TryParse(timeSplitArray[1], out second))
            return false;

        if (timeSplitArray.Length == 3)
        {
            if (!Int64.TryParse(timeSplitArray[2], out millisecond))
                return false;

            if (timeSplitArray[2].Length < 3)
            {
                millisecond = millisecond * 10;
            }
        }

        if (minute < 0 || second < 0 || millisecond < 0)
        {
            LyricLogError("tagString (" + tagString + ") TryParseTimeTag invalid, minute: " + minute + " second: " + second + " millisecond: " + millisecond);
            return false;
        }

        timestamp = minute * 60 * 1000 + second * 1000 + millisecond;
        return true;
    }

    protected void UpdateTimestamp()
    {
        // sort the items with timestamp 
        mItems.Sort();

        int index = 0;
        LyricItem lastItem = null;
        foreach (LyricItem item in mItems)
        {
            // add the timestamp with global offset
            item.mTimestamp += mOffset;
            item.mIndex = index++;

            // update next timestamp
            if (null != lastItem)
            {
                lastItem.mNextTimestamp = item.mTimestamp;
            }
            lastItem = item;
        }
    }

    public void PrintInfo()
    {
        string info = "";
        info += "title: " + mTitle + System.Environment.NewLine;
        info += "artist: " + mArtist + System.Environment.NewLine;
        info += "album: " + mAlbum + System.Environment.NewLine;
        info += "author:" + mAuthor + System.Environment.NewLine;
        info += "by: " + mBy + System.Environment.NewLine;
        info += "offset: " + mOffset + System.Environment.NewLine;
        info += "length: " + mLength + System.Environment.NewLine;
        foreach (LyricItem item in mItems)
        {
            info += string.Format("[index: {0:D3}] ", item.mIndex);
            info += TimestampToString(item.mTimestamp);
            info += item.mText;
            info += "     -> " + TimestampToString(item.mNextTimestamp);
            info += System.Environment.NewLine;
        }
        LyricLog(info);
    }

    public static string TimestampToString(Int64 timestamp)
    {
        if (LyricItem.INVALID_TIME_STAMP == timestamp)
            return "end time stamp";

        return string.Format("[{0:D}:{1:D2}.{2:D3}]", timestamp / (60 * 1000), (timestamp / 1000) % 60, timestamp % 1000);
    }

    protected static void LyricLog(string msg)
    {
        Debug.Log("[Lyric] " + msg);
    }

    protected static void LyricLogDebug(string msg)
    {
        //Debug.Log("[Lyric] " + msg);
    }

    protected static void LyricLogError(string msg)
    {
        Debug.LogError("[Lyric] " + msg);
    }
}
