﻿//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



using System;
using System.IO;

namespace ManagedDoom
{
    public sealed class DemoPlayback
    {
        private Demo demo;
        private TicCmd[] cmds;
        private DoomGame game;

        public DemoPlayback(CommonResource resource, GameOptions options, string demoName)
        {
            if (File.Exists(demoName))
            {
                demo = new Demo(demoName);
            }
            else if (File.Exists(demoName + ".lmp"))
            {
                demo = new Demo(demoName + ".lmp");
            }
            else
            {
                var lumpName = demoName.ToUpper();
                if (resource.Wad.GetLumpNumber(lumpName) == -1)
                {
                    throw new Exception("Demo '" + demoName + "' was not found!");
                }
                demo = new Demo(resource.Wad.ReadLump(lumpName));
            }

            demo.Options.GameMode = options.GameMode;
            demo.Options.MissionPack = options.MissionPack;
            demo.Options.Renderer = options.Renderer;
            demo.Options.Sound = options.Sound;
            demo.Options.Music = options.Music;

            cmds = new TicCmd[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                cmds[i] = new TicCmd();
            }

            game = new DoomGame(resource, demo.Options);
            var skill = demo.Options.Skill;
            var episode = demo.Options.Episode;
            var map = demo.Options.Map;
            game.DeferedInitNew(skill, episode, map);
        }

        public UpdateResult Update()
        {
            if (!demo.ReadCmd(cmds))
            {
                return UpdateResult.Completed;
            }
            else
            {
                return game.Update(cmds);
            }
        }

        public void DoEvent(DoomEvent e)
        {
            game.DoEvent(e);
        }

        public DoomGame Game => game;
    }
}
