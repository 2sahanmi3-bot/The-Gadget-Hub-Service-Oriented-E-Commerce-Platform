namespace TechworldManagementSystem
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Updatebtn = new System.Windows.Forms.Button();
            this.Addbtn = new System.Windows.Forms.Button();
            this.Deletebtn = new System.Windows.Forms.Button();
            this.Thumbnailtxt = new System.Windows.Forms.TextBox();
            this.ThumbnailLbl = new System.Windows.Forms.Label();
            this.ProductCategorytxt = new System.Windows.Forms.TextBox();
            this.ProductCategoryLbl = new System.Windows.Forms.Label();
            this.ProductDetailtxt = new System.Windows.Forms.TextBox();
            this.ProductDetailsLbl = new System.Windows.Forms.Label();
            this.Inventorytxt = new System.Windows.Forms.TextBox();
            this.InventoryLbl = new System.Windows.Forms.Label();
            this.UnitPricetxt = new System.Windows.Forms.TextBox();
            this.UnitPriceLbl = new System.Windows.Forms.Label();
            this.ItemNametxt = new System.Windows.Forms.TextBox();
            this.ItemNameLbl = new System.Windows.Forms.Label();
            this.ProductGlobalIdtxt = new System.Windows.Forms.TextBox();
            this.ProductGlobalLbl = new System.Windows.Forms.Label();
            this.ProductIdtxt = new System.Windows.Forms.TextBox();
            this.IdLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvItems
            // 
            this.dgvItems.BackgroundColor = System.Drawing.Color.Navy;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dgvItems.Location = new System.Drawing.Point(417, 25);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(595, 402);
            this.dgvItems.TabIndex = 60;
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Edit";
            this.Column1.Name = "Column1";
            // 
            // Updatebtn
            // 
            this.Updatebtn.Location = new System.Drawing.Point(317, 446);
            this.Updatebtn.Name = "Updatebtn";
            this.Updatebtn.Size = new System.Drawing.Size(75, 23);
            this.Updatebtn.TabIndex = 59;
            this.Updatebtn.Text = "Update";
            this.Updatebtn.UseVisualStyleBackColor = true;
            this.Updatebtn.Click += new System.EventHandler(this.Updatebtn_Click);
            // 
            // Addbtn
            // 
            this.Addbtn.Location = new System.Drawing.Point(75, 446);
            this.Addbtn.Name = "Addbtn";
            this.Addbtn.Size = new System.Drawing.Size(75, 23);
            this.Addbtn.TabIndex = 58;
            this.Addbtn.Text = "Add";
            this.Addbtn.UseVisualStyleBackColor = true;
            this.Addbtn.Click += new System.EventHandler(this.Addbtn_Click);
            // 
            // Deletebtn
            // 
            this.Deletebtn.Location = new System.Drawing.Point(197, 446);
            this.Deletebtn.Name = "Deletebtn";
            this.Deletebtn.Size = new System.Drawing.Size(75, 23);
            this.Deletebtn.TabIndex = 57;
            this.Deletebtn.Text = "Delete";
            this.Deletebtn.UseVisualStyleBackColor = true;
            this.Deletebtn.Click += new System.EventHandler(this.Deletebtn_Click);
            // 
            // Thumbnailtxt
            // 
            this.Thumbnailtxt.Location = new System.Drawing.Point(188, 362);
            this.Thumbnailtxt.Name = "Thumbnailtxt";
            this.Thumbnailtxt.Size = new System.Drawing.Size(100, 20);
            this.Thumbnailtxt.TabIndex = 56;
            // 
            // ThumbnailLbl
            // 
            this.ThumbnailLbl.AutoSize = true;
            this.ThumbnailLbl.Location = new System.Drawing.Point(90, 365);
            this.ThumbnailLbl.Name = "ThumbnailLbl";
            this.ThumbnailLbl.Size = new System.Drawing.Size(56, 13);
            this.ThumbnailLbl.TabIndex = 55;
            this.ThumbnailLbl.Text = "Thumbnail";
            // 
            // ProductCategorytxt
            // 
            this.ProductCategorytxt.Location = new System.Drawing.Point(188, 316);
            this.ProductCategorytxt.Name = "ProductCategorytxt";
            this.ProductCategorytxt.Size = new System.Drawing.Size(100, 20);
            this.ProductCategorytxt.TabIndex = 54;
            // 
            // ProductCategoryLbl
            // 
            this.ProductCategoryLbl.AutoSize = true;
            this.ProductCategoryLbl.Location = new System.Drawing.Point(90, 319);
            this.ProductCategoryLbl.Name = "ProductCategoryLbl";
            this.ProductCategoryLbl.Size = new System.Drawing.Size(86, 13);
            this.ProductCategoryLbl.TabIndex = 53;
            this.ProductCategoryLbl.Text = "ProductCategory";
            // 
            // ProductDetailtxt
            // 
            this.ProductDetailtxt.Location = new System.Drawing.Point(188, 268);
            this.ProductDetailtxt.Name = "ProductDetailtxt";
            this.ProductDetailtxt.Size = new System.Drawing.Size(100, 20);
            this.ProductDetailtxt.TabIndex = 52;
            // 
            // ProductDetailsLbl
            // 
            this.ProductDetailsLbl.AutoSize = true;
            this.ProductDetailsLbl.Location = new System.Drawing.Point(90, 271);
            this.ProductDetailsLbl.Name = "ProductDetailsLbl";
            this.ProductDetailsLbl.Size = new System.Drawing.Size(76, 13);
            this.ProductDetailsLbl.TabIndex = 51;
            this.ProductDetailsLbl.Text = "ProductDetails";
            // 
            // Inventorytxt
            // 
            this.Inventorytxt.Location = new System.Drawing.Point(188, 223);
            this.Inventorytxt.Name = "Inventorytxt";
            this.Inventorytxt.Size = new System.Drawing.Size(100, 20);
            this.Inventorytxt.TabIndex = 50;
            // 
            // InventoryLbl
            // 
            this.InventoryLbl.AutoSize = true;
            this.InventoryLbl.Location = new System.Drawing.Point(90, 226);
            this.InventoryLbl.Name = "InventoryLbl";
            this.InventoryLbl.Size = new System.Drawing.Size(51, 13);
            this.InventoryLbl.TabIndex = 49;
            this.InventoryLbl.Text = "Inventory";
            // 
            // UnitPricetxt
            // 
            this.UnitPricetxt.Location = new System.Drawing.Point(188, 181);
            this.UnitPricetxt.Name = "UnitPricetxt";
            this.UnitPricetxt.Size = new System.Drawing.Size(100, 20);
            this.UnitPricetxt.TabIndex = 48;
            // 
            // UnitPriceLbl
            // 
            this.UnitPriceLbl.AutoSize = true;
            this.UnitPriceLbl.Location = new System.Drawing.Point(90, 184);
            this.UnitPriceLbl.Name = "UnitPriceLbl";
            this.UnitPriceLbl.Size = new System.Drawing.Size(53, 13);
            this.UnitPriceLbl.TabIndex = 47;
            this.UnitPriceLbl.Text = "Unit Price";
            // 
            // ItemNametxt
            // 
            this.ItemNametxt.Location = new System.Drawing.Point(188, 137);
            this.ItemNametxt.Name = "ItemNametxt";
            this.ItemNametxt.Size = new System.Drawing.Size(100, 20);
            this.ItemNametxt.TabIndex = 46;
            // 
            // ItemNameLbl
            // 
            this.ItemNameLbl.AutoSize = true;
            this.ItemNameLbl.Location = new System.Drawing.Point(90, 140);
            this.ItemNameLbl.Name = "ItemNameLbl";
            this.ItemNameLbl.Size = new System.Drawing.Size(58, 13);
            this.ItemNameLbl.TabIndex = 45;
            this.ItemNameLbl.Text = "Item Name";
            // 
            // ProductGlobalIdtxt
            // 
            this.ProductGlobalIdtxt.Location = new System.Drawing.Point(188, 94);
            this.ProductGlobalIdtxt.Name = "ProductGlobalIdtxt";
            this.ProductGlobalIdtxt.Size = new System.Drawing.Size(100, 20);
            this.ProductGlobalIdtxt.TabIndex = 44;
            // 
            // ProductGlobalLbl
            // 
            this.ProductGlobalLbl.AutoSize = true;
            this.ProductGlobalLbl.Location = new System.Drawing.Point(90, 97);
            this.ProductGlobalLbl.Name = "ProductGlobalLbl";
            this.ProductGlobalLbl.Size = new System.Drawing.Size(91, 13);
            this.ProductGlobalLbl.TabIndex = 43;
            this.ProductGlobalLbl.Text = "Product Global ID";
            // 
            // ProductIdtxt
            // 
            this.ProductIdtxt.Location = new System.Drawing.Point(188, 50);
            this.ProductIdtxt.Name = "ProductIdtxt";
            this.ProductIdtxt.Size = new System.Drawing.Size(100, 20);
            this.ProductIdtxt.TabIndex = 42;
            // 
            // IdLbl
            // 
            this.IdLbl.AutoSize = true;
            this.IdLbl.Location = new System.Drawing.Point(90, 53);
            this.IdLbl.Name = "IdLbl";
            this.IdLbl.Size = new System.Drawing.Size(58, 13);
            this.IdLbl.TabIndex = 41;
            this.IdLbl.Text = "Product ID";
            this.IdLbl.Click += new System.EventHandler(this.ProductIdLbl_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(1024, 549);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.Updatebtn);
            this.Controls.Add(this.Addbtn);
            this.Controls.Add(this.Deletebtn);
            this.Controls.Add(this.Thumbnailtxt);
            this.Controls.Add(this.ThumbnailLbl);
            this.Controls.Add(this.ProductCategorytxt);
            this.Controls.Add(this.ProductCategoryLbl);
            this.Controls.Add(this.ProductDetailtxt);
            this.Controls.Add(this.ProductDetailsLbl);
            this.Controls.Add(this.Inventorytxt);
            this.Controls.Add(this.InventoryLbl);
            this.Controls.Add(this.UnitPricetxt);
            this.Controls.Add(this.UnitPriceLbl);
            this.Controls.Add(this.ItemNametxt);
            this.Controls.Add(this.ItemNameLbl);
            this.Controls.Add(this.ProductGlobalIdtxt);
            this.Controls.Add(this.ProductGlobalLbl);
            this.Controls.Add(this.ProductIdtxt);
            this.Controls.Add(this.IdLbl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewButtonColumn Column1;
        private System.Windows.Forms.Button Updatebtn;
        private System.Windows.Forms.Button Addbtn;
        private System.Windows.Forms.Button Deletebtn;
        private System.Windows.Forms.TextBox Thumbnailtxt;
        private System.Windows.Forms.Label ThumbnailLbl;
        private System.Windows.Forms.TextBox ProductCategorytxt;
        private System.Windows.Forms.Label ProductCategoryLbl;
        private System.Windows.Forms.TextBox ProductDetailtxt;
        private System.Windows.Forms.Label ProductDetailsLbl;
        private System.Windows.Forms.TextBox Inventorytxt;
        private System.Windows.Forms.Label InventoryLbl;
        private System.Windows.Forms.TextBox UnitPricetxt;
        private System.Windows.Forms.Label UnitPriceLbl;
        private System.Windows.Forms.TextBox ItemNametxt;
        private System.Windows.Forms.Label ItemNameLbl;
        private System.Windows.Forms.TextBox ProductGlobalIdtxt;
        private System.Windows.Forms.Label ProductGlobalLbl;
        private System.Windows.Forms.TextBox ProductIdtxt;
        private System.Windows.Forms.Label IdLbl;
    }
}

