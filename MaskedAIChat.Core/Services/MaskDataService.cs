﻿using System.Text.RegularExpressions;
using MaskedAIChat.Core.Contracts.Services;
using MaskedAIChat.Core.Models;

namespace MaskedAIChat.Core.Services;

// Holds data for all the masks
public class MaskDataService : IMaskDataService
{
    private List<Mask> _allMasks;
    private int _maskCount;

    public MaskDataService()
    {
        _allMasks ??= new List<Mask>(AllMasks());
        _maskCount = _allMasks.Count;
    }

    public IEnumerable<Mask> GetMasks()
    {
        return _allMasks;
    }

    private static IEnumerable<Mask> AllMasks()
    {
        // The following is example data
        var masks = new List<Mask>()
        {
            //new Mask()
            //{
            //    MaskID = 1,
            //    UnmaskedText = "read@substack.com",
            //    MaskedText = "asdfadsfasdfadsf@example.com"

            //}
        };
        return masks;
    }

    public string MaskText(string textToMask)
    {
        var maskedText = textToMask;
        foreach (var mask in _allMasks)
        {
            if (mask.UnmaskedText != null)
            {
                maskedText = maskedText.Replace(mask.UnmaskedText, mask.MaskedText);
            }
        }

        return maskedText;
    }

    /* Add masks for every e-Mail you find
     * 
     */
    public void BuildMasks(string textToParse)
    {
        Regex regex = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b");
        MatchCollection matches = regex.Matches(textToParse);
        foreach (Match match in matches)
        {
            var found = false;
            foreach (var mask in _allMasks)
            {
                if (found) break;
                else found = mask.UnmaskedText.Equals(match.Value);

            }
            if (!found)
            {
                var newMaskID = _allMasks.Count + 1;
                _allMasks.Add(new Mask()
                {
                    MaskID = newMaskID,
                    UnmaskedText = match.Value,
                    MaskedText = "Masked" + newMaskID + "@example.com"
                }
                );
            }
        }
    }

    public async Task<IEnumerable<Mask>> GetContentGridDataAsync()
    {
        await Task.CompletedTask;
        return _allMasks;
    }

    public async Task<IEnumerable<Mask>> GetGridDataAsync()
    {
        await Task.CompletedTask;
        return _allMasks;
    }

    public async Task<IEnumerable<Mask>> GetListDetailsDataAsync()
    {
        await Task.CompletedTask;
        return _allMasks;
    }
}
