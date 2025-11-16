// GadgetHubClient JavaScript Functions
// Enhanced error handling and user feedback

// Global error handler
window.addEventListener('error', function(event) {
    console.error('Global error:', event.error);
    showNotification('An unexpected error occurred. Please refresh the page.', 'error');
});

// Global unhandled promise rejection handler
window.addEventListener('unhandledrejection', function(event) {
    console.error('Unhandled promise rejection:', event.reason);
    showNotification('Network error occurred. Please check your connection.', 'error');
});

// Enhanced notification system
function showNotification(message, type = 'info', duration = 5000) {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.notification');
    existingNotifications.forEach(notification => notification.remove());

    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas fa-${getNotificationIcon(type)}"></i>
            <span>${message}</span>
            <button class="notification-close" onclick="this.parentElement.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        background: ${getNotificationColor(type)};
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        max-width: 400px;
        animation: slideInRight 0.3s ease-out;
    `;

    // Add animation styles
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        .notification-content {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        .notification-close {
            background: none;
            border: none;
            color: white;
            cursor: pointer;
            margin-left: auto;
        }
    `;
    document.head.appendChild(style);

    document.body.appendChild(notification);

    // Auto remove after duration
    setTimeout(() => {
        if (notification.parentElement) {
            notification.remove();
        }
    }, duration);
}

function getNotificationIcon(type) {
    switch (type) {
        case 'success': return 'check-circle';
        case 'error': return 'exclamation-circle';
        case 'warning': return 'exclamation-triangle';
        case 'info': return 'info-circle';
        default: return 'info-circle';
    }
}

function getNotificationColor(type) {
    switch (type) {
        case 'success': return '#10b981';
        case 'error': return '#ef4444';
        case 'warning': return '#f59e0b';
        case 'info': return '#3b82f6';
        default: return '#6b7280';
    }
}

// Enhanced API call helper
async function makeApiCall(url, options = {}) {
    try {
        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error('API call failed:', error);
        throw error;
    }
}

// Enhanced product search functionality
function searchProducts() {
    const searchTerm = document.getElementById('searchInput')?.value?.toLowerCase() || '';
    const productItems = document.querySelectorAll('.product-item');
    
    productItems.forEach(item => {
        const productName = item.dataset.name || '';
        const matches = productName.includes(searchTerm);
        item.style.display = matches ? 'block' : 'none';
    });
    
    // Show message if no results
    const visibleProducts = document.querySelectorAll('.product-item[style*="block"], .product-item:not([style])');
    if (visibleProducts.length === 0 && searchTerm) {
        showNotification('No products found matching your search.', 'info');
    }
}

// Enhanced filter functionality
function filterProducts() {
    const selectedCategory = document.getElementById('categoryFilter')?.value?.toLowerCase() || '';
    const productItems = document.querySelectorAll('.product-item');
    
    productItems.forEach(item => {
        const productCategory = item.dataset.category || '';
        const matches = !selectedCategory || productCategory === selectedCategory;
        item.style.display = matches ? 'block' : 'none';
    });
}

// Enhanced view product details function
function viewProductDetails(productId, productName) {
    if (!productId) {
        showNotification('Invalid product ID. Please try again.', 'error');
        return;
    }
    
    // Navigate to the product details page
    window.location.href = `/ProductDetails/${productId}`;
}

// Loading state management
function setLoadingState(elementId, isLoading) {
    const element = document.getElementById(elementId);
    if (!element) return;
    
    if (isLoading) {
        element.style.opacity = '0.6';
        element.style.pointerEvents = 'none';
    } else {
        element.style.opacity = '1';
        element.style.pointerEvents = 'auto';
    }
}

// Form validation helper
function validateForm(formElement) {
    const requiredFields = formElement.querySelectorAll('[required]');
    let isValid = true;
    
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
        }
    });
    
    return isValid;
}

// Initialize page functionality
document.addEventListener('DOMContentLoaded', function() {
    // Initialize search functionality
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', searchProducts);
    }
    
    // Initialize filter functionality
    const categoryFilter = document.getElementById('categoryFilter');
    if (categoryFilter) {
        categoryFilter.addEventListener('change', filterProducts);
    }
    
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    console.log('GadgetHubClient JavaScript initialized successfully');
});