using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockFaq1ViewModel : ContentBlockViewModel<ContentBlockFaq1>
    {
        public IEnumerable<FAQCategoryQuestions> FAQ { get; }

        public ContentBlockFaq1ViewModel(ContentBlockFaq1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            var result = new List<FAQCategoryQuestions>();

            var categories = content.Faq.GroupBy(faq =>
            {
                if (faq is Faqquestion)
                {
                    return faq.Parent.Id;
                }
                else
                {
                    // Faqcategory
                    return faq.Id;
                }
            });

            FAQ = categories.Select(g =>
            {
                Faqcategory category = g.FirstOrDefault(ipc => ipc is Faqcategory) as Faqcategory;
                IEnumerable<Faqquestion> questions;

                if (category != null)
                {
                    questions = category.Children<Faqquestion>().ToList();
                }
                else
                {
                    category = g.FirstOrDefault()?.Parent<Faqcategory>();
                    questions = g.Select(ipc => new Faqquestion(ipc)).ToList();
                }

                return new FAQCategoryQuestions
                {
                    FAQCategory = category,
                    FAQQuestions = questions
                };
            });
        }
    }

    public class FAQCategoryQuestions
    {
        public Faqcategory FAQCategory { get; set; }
        public IEnumerable<Faqquestion> FAQQuestions { get; set; }
    }
}
