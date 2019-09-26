// <copyright file="IGenStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    public interface IGenStep
    {
        bool CanApply(IGenContext context);

        void Apply(IGenContext context);
    }
}
