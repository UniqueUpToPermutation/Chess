using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Chess.Graphics
{
    public class RenderScene : IDisposable
    {
        protected List<Light> lights = new List<Light>();
        protected Dictionary<string, Material> materials = new Dictionary<string, Material>();
        protected Dictionary<string, Model> models = new Dictionary<string, Model>();
        protected Dictionary<Model, List<RenderNode>> renderNodes = new Dictionary<Model, List<RenderNode>>();

        public Dictionary<Model, List<RenderNode>> RenderNodes { get { return renderNodes; } }
        public Dictionary<string, Material> Materials { get { return materials; } }
        public Dictionary<string, Model> Models { get { return models; } }
        public List<Light> Lights { get { return lights; } }

        public void AddModel(Model model)
        {
            if (models.ContainsKey(model.Name))
                models[model.Name] = model;
            else
                models.Add(model.Name, model);
        }

        public void AddMaterial(Material material)
        {
            if (materials.ContainsKey(material.Name))
                materials[material.Name] = material;
            else
                materials.Add(material.Name, material);
        }

        public void AddLight(Light light)
        {
            Lights.Add(light);
        }

        public void AddRenderNode(RenderNode node)
        {
            List<RenderNode> list;
            if (renderNodes.TryGetValue(node.Model, out list))
                list.Add(node);
            else
                renderNodes.Add(node.Model, new List<RenderNode>() { node });
        }

        public virtual void Update(FrameEventArgs e)
        {
            foreach (var material in materials.Values)
                material.Update(e);
        }

        public void RemoveRenderNode(RenderNode node)
        {
            List<RenderNode> list;
            if (renderNodes.TryGetValue(node.Model, out list))
            {
                list.Remove(node);
                if (list.Count == 0)
                    renderNodes.Remove(node.Model);
            }
        }

        public void Dispose()
        {
            foreach (Light l in lights)
                l.Dispose();
        }
    }
}
