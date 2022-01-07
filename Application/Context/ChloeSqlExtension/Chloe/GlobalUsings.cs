global using System;
global using System.Text;
global using System.Linq;
global using System.IO;
global using System.Collections.Generic;

#if netfx
global using BoolResultTask = System.Threading.Tasks.Task<bool>;
#else
global using BoolResultTask = System.Threading.Tasks.ValueTask<bool>;
#endif
