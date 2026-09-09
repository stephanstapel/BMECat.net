/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System.IO;

namespace BMECat.net
{
    internal static class StreamUtils
    {
        /// <summary>
        /// Reads from the stream until the buffer is filled or the end of the stream is reached.
        ///
        /// A single call to Stream.Read() may return less bytes than requested, e.g. network streams
        /// or pipes hand out their data in chunks. Relying on one read call would truncate the data.
        /// </summary>
        /// <returns>the number of bytes that were actually read</returns>
        internal static int FillBuffer(Stream stream, byte[] buffer)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < buffer.Length)
            {
                int bytesRead = stream.Read(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                if (bytesRead <= 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        } // !FillBuffer()
    }
}
