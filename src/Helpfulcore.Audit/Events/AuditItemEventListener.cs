using System;
using System.Text;
using Helpfulcore.Audit.Formatters;
using Helpfulcore.Logging;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Events;

namespace Helpfulcore.Audit.Events
{
    public class AuditItemEventListener
    {
        protected ILoggingService Log
        {
            get { return Factory.CreateObject("helpfulcore/audit/loggingService", true) as ILoggingService; }
        }

        protected FieldChangesFormatter FieldChangesFormatter
        {
            get { return Factory.CreateObject("helpfulcore/audit/fieldChangesFormatter", true) as FieldChangesFormatter; }
        }

        public void OnItemAdded(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex); 
            }
        }

        public void OnItemCreated(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var itemCreated = ExtractArgument<ItemCreatedEventArgs>(args, 0);
                this.LogAuditMessage(eventArgs.EventName, itemCreated.Item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemRenamed(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var newName = ExtractArgument<string>(args, 1);
                this.Log.Audit($"{eventArgs.EventName} {this.UserAndItemInfo(item)}. New item name : '{newName}'", this);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemCopied(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var itemCopy = ExtractItem(args, 1);
                this.Log.Audit($"{eventArgs.EventName} {this.UserInfo()}. Copy of {ItemInfo(itemCopy)}, Original: {ItemInfo(item)}", this);

                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemCopying(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var targetItem = ExtractArgument<Item>(args, 1);
                var copyName = ExtractArgument<string>(args, 2);
                var copyId = ExtractArgument<ID>(args, 3);
                this.Log.Audit($"{eventArgs.EventName} {this.UserAndItemInfo(item)}. Target parent item: {targetItem.ID}, copy name: {copyName}, copyID: {copyId}", this);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemCloneAdded(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemCreating(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var itemCreating = ExtractArgument<ItemCreatingEventArgs>(args, 0);
                this.Log.Audit($"{eventArgs.EventName} {this.UserInfo()} Attempt to create an item with name '{itemCreating.ItemName}' of template '{itemCreating.TemplateId}' with id:{itemCreating.ItemId}. Parent {this.ItemInfo(itemCreating.Parent)} ", this);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemDeleting(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemMoved(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var targetParent = ExtractArgument<ID>(args, 1);
                this.Log.Audit($"{eventArgs.EventName} {this.UserAndItemInfo(item)}. Target parent item: {targetParent}", this);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemMoving(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var oldParent = ExtractArgument<ID>(args, 1);
                var newParent = ExtractArgument<ID>(args, 2);
                this.Log.Audit($"{eventArgs.EventName} {this.UserAndItemInfo(item)}. Original parent item: {oldParent}, new parent item: {newParent}", this);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var itemChanges = ExtractArgument<ItemChanges>(args, 1);

                var formattedChanges = this.FieldChangesFormatter.Format(itemChanges);

                this.Log.Audit($"{eventArgs.EventName} {this.UserAndItemInfo(item)}. {formattedChanges}", this);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemSaving(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemTemplateChanged(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                var templateChange = ExtractArgument<TemplateChangeList>(args, 1);
                var oldTemplate = templateChange.Source;
                var newTemplate = templateChange.Target;

                this.Log.Audit($"{eventArgs.EventName} {this.UserAndItemInfo(item)}. Original template: {oldTemplate.ID}, new template: {newTemplate}", this);

                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemTransferred(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemVersionAdding(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        public void OnItemVersionRemoving(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            try
            {
                var item = ExtractItem(args, 0);
                this.LogAuditMessage(eventArgs.EventName, item);
            }
            catch (Exception ex)
            {
                this.LogException(eventArgs?.EventName, ex);
            }
        }

        private static Item ExtractItem(EventArgs args, int itemParamIndex)
        {
            return Event.ExtractParameter(args, itemParamIndex) as Item;
        }

        private static T ExtractArgument<T>(EventArgs args, int itemParamIndex)
        {
            return (T)Event.ExtractParameter(args, itemParamIndex);
        }

        private void LogException(string eventName, Exception ex)
        {
            this.Log.Error($"Error while executing audit logging on {eventName} event handler. {ex.Message}", this, ex);
        }

        private void LogAuditMessage(string message, Item item)
        {
            this.Log.Audit($"{message} {this.UserAndItemInfo(item)}", this);
        }

        private string UserInfo()
        {
            return $"({Sitecore.Context.User?.Name})";
        }

        private string ItemInfo(Item item)
        {
            return $"item: lang:{item?.Language?.Name} '{item?.Paths?.FullPath}' id:{item?.ID}";
        }

        private string UserAndItemInfo(Item item)
        {
            return $"{this.UserInfo()} {this.ItemInfo(item)}";
        }
    }
}
